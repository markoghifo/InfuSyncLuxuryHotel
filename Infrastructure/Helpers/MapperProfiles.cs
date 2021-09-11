using AutoMapper;
using Core.Models;
using Infrastructure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public class MapperProfiles: Profile
    {
        public MapperProfiles()
        {
            CreateMap<Client, ClientDTO>().ReverseMap();
            CreateMap<Room, RoomDTO>().ReverseMap();
            CreateMap<Room, RegisterRoomDTO>().ReverseMap();
            CreateMap<RoomBookingHistory, BookRoomDTO>().ReverseMap();
            CreateMap<RoomBookingHistory, RoomBookingHistoryDTO>().ReverseMap();
        }
        
    }
}
