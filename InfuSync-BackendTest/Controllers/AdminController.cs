using Core.DI;
using Core.Models;
using Hangfire;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InfuSync_BackendTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IRoomService roomService,
            ILogger<AdminController> logger
            )
        {
            _logger = logger;
            _roomService = roomService;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<RoomDTO>>> GetRooms()
        {
            return Ok(ResponseUtil<IEnumerable<RoomDTO>>.Ok(_roomService.GetRooms(null)));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RegisterRoomDTO>>> RegisterRoom([FromBody] RegisterRoomDTO value)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Bad Request", value);
                return BadRequest(ResponseUtil<string>.Error(ModelState.GetErrorMessage(), "Bad Request"));
            }
            await _roomService.RegisterRoomAsync(value);
            return CreatedAtRoute("ViewRooms", null, ResponseUtil<RegisterRoomDTO>.Ok(value));
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<RoomBookingHistoryDTO>>> UpdateBookingStatus(string bookingId, [FromBody] BookRoomDTO value)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Bad Request", value);
                return BadRequest(ResponseUtil<string>.Error(ModelState.GetErrorMessage(), "Bad Request"));
            }
            var resp = await _roomService.UpdateBookingAsync(bookingId, value);
            return Ok(ResponseUtil<RoomBookingHistoryDTO>.Ok(resp));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<CustomerBookingDataDTO>>> GetCustomerBookingData(string clientId, int roomNumber, DateTime checkInDate)
        {
            var res = await _roomService.GetCustomerBookingData(clientId, roomNumber, checkInDate);
            return Ok(ResponseUtil<CustomerBookingDataDTO>.Ok(res));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<DashboardDataDTO>>> GetDashboardData()
        {
            var bookings = _roomService.GetRoomBookingStatisticsAsync();
            var rooms = _roomService.GetRoomStatisticsAsync();
            var revenue = _roomService.GetTotalRevenue();
            await Task.WhenAll(bookings, rooms, revenue);
            return Ok(ResponseUtil<DashboardDataDTO>.Ok(new DashboardDataDTO
            {
                checkIns = bookings.Result.FindAll(b => b.State == ((int)RoomBookingActions.CheckIn).ToString()).Count(),
                checkOuts = bookings.Result.FindAll(b => b.State == ((int)RoomBookingActions.CheckOut).ToString()).Count(),
                freeRooms = rooms.Result.Count(r => r.State == ((int)RoomStates.Free).ToString()),
                occupied = rooms.Result.Count(r => r.State == ((int)RoomStates.Occupied).ToString()),
                revenue = revenue.Result,
                totalRooms = rooms.Result.Count()
            }));
        }
    }
}
