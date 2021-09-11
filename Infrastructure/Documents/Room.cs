using Core.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Documents
{
    public class Room
    {
        public Room()
        {
            this.State = RoomStates.Free;
            this.CreateDate = DateTime.Now;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int RoomNumber { get; set; }
        public string[] Features { get; set; }
        public decimal Price { get; set; }
        public int MaxNumberOfPersons { get; set; }
        public string Category { get; set; }
        public RoomStates State { get; set; }
        //public DateTime DateBooked { get; set; }
        //public DateTime CheckInDate { get; set; }
        //public DateTime CheckOutDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
