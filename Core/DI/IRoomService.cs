using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DI
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDTO>> RegisterRoomAsync(RegisterRoomDTO data);
        Task<RoomBookingHistoryDTO> InitiateBookingAsync(BookRoomDTO data);
        Task<RoomBookingHistoryDTO> UpdateBookingAsync(string id, BookRoomDTO data);
        RoomDTO Get(string id);
        IEnumerable<RoomDTO> GetRooms(int? roomNumber);
        Task UpdateAsync(string id, RoomDTO clientIn);
        Task<List<RoomBookingStatisticsDTO>> GetRoomBookingStatisticsAsync();
        Task<List<RoomStatisticsDTO>> GetRoomStatisticsAsync();
        Task<decimal> GetTotalRevenue();
        Task<CustomerBookingDataDTO> GetCustomerBookingData(string clientId, int roomNumber, DateTime checkInDate);
    }
}
