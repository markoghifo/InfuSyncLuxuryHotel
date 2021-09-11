using AutoMapper;
using Core.DI;
using Core.Models;
using Infrastructure.Documents;
using Infrastructure.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class RoomService : IRoomService
    {
        private readonly IMongoCollection<Room> _rooms;
        private readonly IMongoCollection<RoomBookingHistory> _roomBookingHist;
        private readonly IMapper _mapper;
        private readonly IClientService _clientService;

        public RoomService(IHotelDatabaseSettings settings, IMapper mapper, IClientService clientService)
        {
            _mapper = mapper;
            _clientService = clientService;

            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _rooms = database.GetCollection<Room>("Rooms");
            _roomBookingHist = database.GetCollection<RoomBookingHistory>("RoomBookingHistory");
        }

        public RoomDTO Get(string id)
        {
            var room = _rooms.Find<Room>(r => r.Id == id).FirstOrDefault();
            return EntityMapper.Mapper.Map<RoomDTO>(room);
        }

        public IEnumerable<RoomDTO> GetRooms(int? roomNumber)
        {
            var rooms = new List<Room>();
            if (roomNumber != null)
            {
                rooms = _rooms.Find(r => r.RoomNumber == roomNumber).ToList();
            }
            else
            {
                rooms = _rooms.Find(r => true).ToList();
            }
            var res = Helpers.EntityMapper.Mapper.Map<List<RoomDTO>>(rooms);
            return res;
        }

        public async Task<IEnumerable<RoomDTO>> RegisterRoomAsync(RegisterRoomDTO data)
        {
            foreach (var item in data.AssociatedRoomNumbers)
            {
                var roomDto = GetRooms(item).FirstOrDefault();
                if (roomDto == null)
                {
                    var rm = new Room
                    {
                        RoomNumber = item,
                        Features = data.Features,
                        Category = data.Category,
                        MaxNumberOfPersons = data.MaxNumberOfPersons,
                        Price = data.Price,
                    };
                    await _rooms.InsertOneAsync(rm);
                }
            }
            var res = EntityMapper.Mapper.Map<IEnumerable<RoomDTO>>(data);
            return res;
        }

        public async Task<RoomBookingHistoryDTO> InitiateBookingAsync(BookRoomDTO data)
        {
            var room = Get(data.RoomId);
            if (room != null)
            {
                var hist = EntityMapper.Mapper.Map<RoomBookingHistory>(data);
                hist.Action = RoomBookingActions.Booking;
                hist.ClientId = ""; //get from user claims
                hist.TotalCost = room.Price;
                hist.Services = new RoomServices[] { };

                await _roomBookingHist.InsertOneAsync(hist);

                //Update client state to booked room
                await _clientService.UpdateClientStateAsync(hist.ClientId, ClientStates.BookedRoom);

                //Update room state
                room.State = RoomStates.Reserved;
                await UpdateAsync(room.Id, room);
                return EntityMapper.Mapper.Map<RoomBookingHistoryDTO>(hist);
            }
            throw new Exception("Room not found");
        }

        public async Task<RoomBookingHistoryDTO> UpdateBookingAsync(string id, BookRoomDTO data)
        {
            var room = Get(data.RoomId);
            if (room != null)
            {
                var hist = EntityMapper.Mapper.Map<RoomBookingHistory>(data);
                var filter = Builders<RoomBookingHistory>.Filter.Eq("_id", id);
                var update = Builders<RoomBookingHistory>.Update
                    .Set(nameof(data.Action), data.Action)
                    .Set(nameof(data.CheckInDateTime), data.CheckInDateTime)
                    .Set(nameof(data.CheckOutDateTime), data.CheckOutDateTime);
                await _roomBookingHist.UpdateOneAsync(filter, update);

                //create audit trail for booking updates


                //Update room and Client state - Can use StateMachine
                RoomStates roomState = RoomStates.Free;
                ClientStates clientState = ClientStates.Registered;
                switch (data.Action)
                {
                    case RoomBookingActions.CheckIn:
                        roomState = RoomStates.Occupied;
                        clientState = ClientStates.CheckIn;
                        break;
                    case RoomBookingActions.CheckOut:
                        roomState = RoomStates.Free;
                        clientState = ClientStates.CheckOut;
                        break;
                    case RoomBookingActions.Booking:
                        roomState = RoomStates.Reserved;
                        clientState = ClientStates.BookedRoom;
                        break;
                    default:
                        break;
                }
                room.State = roomState;

                var clientId = ""; //based on userClaim

                await Task.WhenAll(UpdateAsync(room.Id, room), _clientService.UpdateClientStateAsync(clientId, clientState));

                return EntityMapper.Mapper.Map<RoomBookingHistoryDTO>(hist);
            }
            throw new Exception("Room not found");
        }

        public async Task<CustomerBookingDataDTO> GetCustomerBookingData(string clientId, int roomNumber, DateTime checkInDate)
        {
            var booking = await _roomBookingHist.FindAsync(x => x.CheckInDateTime.Value.ToShortDateString() == checkInDate.ToShortDateString()
                                    && x.ClientId == clientId
                                    && x.RoomNumber == roomNumber);
            var res = booking.ToList().FirstOrDefault();

            if(res == null)
                return null;

            return new CustomerBookingDataDTO
            {
                NumberOfDaysUtilized = (res.CheckOutDateTime.Value - res.CheckInDateTime.Value).Days,
                ServicesConsumed = res.Services,
                TotalCost = res.TotalCost
            };
        }

        public async Task<List<RoomStatisticsDTO>> GetRoomStatisticsAsync()
        {
            var group = new BsonDocument
            {
                {
                    "$group",
                    new BsonDocument
                    {
                        {"_id", "State"},
                        { "counter", new BsonDocument
                            {
                                {"$addToSet", "$RoomNumber"}
                            }
                        }
                    }
                }
            };
            var project = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {"_id", "0"},
                        { "State", "$_id"},
                        {"Counter", new BsonDocument
                            {
                                {
                                    "$size", "$counter"
                                }
                            }
                        }
                    }
                }
            };

            var pipeline = new[] { group, project };
            var result = await _rooms.AggregateAsync<RoomStatisticsDTO>(pipeline);
            return await result.ToListAsync();
        }

        public async Task<List<RoomBookingStatisticsDTO>> GetRoomBookingStatisticsAsync()
        {
            var group = new BsonDocument
            {
                {
                    "$group",
                    new BsonDocument
                    {
                        {"_id", "Action"},
                        { "counter", new BsonDocument
                            {
                                {"$addToSet", "$_id"}
                            }
                        }
                    }
                }
            };
            var project = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {"_id", "0"},
                        { "Action", "$_id"},
                        {"Counter", new BsonDocument
                            {
                                {
                                    "$size", "$counter"
                                }
                            }
                        }
                    }
                }
            };

            var pipeline = new[] { group, project };
            var result = await _roomBookingHist.AggregateAsync<RoomBookingStatisticsDTO>(pipeline);
            return await result.ToListAsync();
        }

        public async Task<decimal> GetTotalRevenue()
        {
            var project = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {"_id", "0"},
                        {"Revenue", new BsonDocument
                            {
                                {
                                    "$sum", "$TotalCost"
                                }
                            }
                        }
                    }
                }
            };

            var pipeline = new[] { project };
            var result = await _roomBookingHist.AggregateAsync<RoomBookingRevenueDTO>(pipeline);
            var res = result.FirstOrDefault();
            return res != null ? res.Revenue : 0;
        }

        public async Task UpdateAsync(string id, RoomDTO roomIn)
        {
            var room = EntityMapper.Mapper.Map<Room>(roomIn);
            await _rooms.ReplaceOneAsync(r => r.Id == id, room);
        }

        public void Remove(RoomDTO roomIn) =>
            _rooms.DeleteOne(r => r.Id == roomIn.Id);

        public void Remove(string id) =>
            _rooms.DeleteOne(r => r.Id == id);
    }
}
