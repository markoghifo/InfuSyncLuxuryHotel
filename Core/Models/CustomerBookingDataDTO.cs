using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CustomerBookingDataDTO
    {
        public int NumberOfDaysUtilized { get; set; }
        public RoomServices[] ServicesConsumed { get; set; }
        public decimal TotalCost { get; set; }
    }
}
