using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class RoomBookingHistoryDTO
    {
        public string Id { get; set; }
        public string RoomId { get; set; }
        public int RoomNumber { get; set; }
        public int NumberOfPersonsInRoom { get; set; }
        public DateTime? CheckInDateTime { get; set; }
        public DateTime? CheckOutDateTime { get; set; }
        public DateTime DateBooked { get; set; }
        public RoomBookingActions Action { get; set; }
        public RoomServices[] Services { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class RoomServices
    {
        public string ServiceName { get; set; }
        public DateTime DateServed { get; set; }
        public decimal ServiceCost { get; set; }
    }
}
