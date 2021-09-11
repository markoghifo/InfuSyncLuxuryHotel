using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class RoomDTO
    {
        public string Id { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public RoomStates State { get; set; }
    }

    public enum RoomStates
    {
        Free, Occupied, Reserved, Cleaned, Unavailable
    }

    public class RegisterRoomDTO
    {
        [Required]
        public string Category { get; set; }
        [Required]
        public int[] AssociatedRoomNumbers { get; set; }
        [Required]
        public string[] Features { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int MaxNumberOfPersons { get; set; }
    }

    public enum RoomCategories
    {
        Standard, Deluxe, VIP, Presidential
    }
}
