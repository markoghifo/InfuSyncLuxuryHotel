using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class DashboardDataDTO
    {
        public int totalRooms { get; set; }
        public int occupied { get; set; }
        public int freeRooms { get; set; }
        public decimal revenue { get; set; }
        public long checkIns { get; set; }
        public long checkOuts { get; set; }
    }
}
