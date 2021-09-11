using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class BookRoomDTO
    {
        public string Id { get; set; }
        [Required]
        public string RoomId { get; set; }
        [Required]
        public int RoomNumber { get; set; }
        public int NumberOfPersonsInRoom { get; set; }
        [Required]
        public string ClientId { get; set; }
        public DateTime? CheckInDateTime { get; set; }
        public DateTime? CheckOutDateTime { get; set; }
        [Required]
        public RoomBookingActions Action { get; set; }
    }

    public enum RoomBookingActions
    {
        CheckIn, CheckOut, Booking
    }
}
