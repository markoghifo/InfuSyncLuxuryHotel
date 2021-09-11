using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class RoomStatisticsDTO
    {
        public int Counter { get; set; }
        public string State { get; set; }
    }

    public class RoomBookingStatisticsDTO
    {
        public int Counter { get; set; }
        public string State { get; set; }
    }

    public class RoomBookingRevenueDTO
    {
        public decimal Revenue { get; set; }
    }
}
