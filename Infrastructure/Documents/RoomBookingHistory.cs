using Core.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Documents
{
    public class RoomBookingHistory
    {
        public RoomBookingHistory()
        {
            DateBooked = DateTime.Now;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string RoomId { get; set; }
        public int RoomNumber { get; set; }
        public int NumberOfPersonsInRoom { get; set; }
        public string ClientId { get; set; }
        public DateTime? CheckInDateTime { get; set; }
        public DateTime? CheckOutDateTime { get; set; }
        public DateTime DateBooked { get; set; }
        public RoomBookingActions Action { get; set; }
        public RoomServices[] Services { get; set; }
        public decimal TotalCost { get; set; }
    }

}
