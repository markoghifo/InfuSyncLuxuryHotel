using Core.DI;
using Core.Models;
using Hangfire;
using Infrastructure.Documents;
using Infrastructure.Helpers;
using Infrastructure.Services;
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
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IRoomService _roomService;
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(ILogger<ClientsController> logger,
            IClientService clientService, 
            IRoomService roomService, 
            IBackgroundJobClient backgroundJobs)
        {
            _clientService = clientService;
            _roomService = roomService;
            _backgroundJobs = backgroundJobs;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ClientDTO>>> RegisterAsync([FromBody] ClientDTO value)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Bad Request", value);
                return BadRequest(ResponseUtil<string>.Error(ModelState.GetErrorMessage(), "Bad Request"));
            }
            var client = await _clientService.CreateAsync(value);
            _backgroundJobs.Enqueue<IEmailSender>(x => x.Send("PROFILE SETUP", "", ""));
            return CreatedAtRoute("ViewProfile", new { id = value.Id.ToString() }, ResponseUtil<ClientDTO>.Ok(client));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoomBookingHistoryDTO>>> BookRoom([FromBody] BookRoomDTO value)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Bad Request", value);
                return BadRequest(ResponseUtil<string>.Error(ModelState.GetErrorMessage(), "Bad Request"));
            }
            var booking = await _roomService.InitiateBookingAsync(value);
            _backgroundJobs.Enqueue<IEmailSender>(x => x.Send("ROOM BOOKING", "", ""));
            return CreatedAtRoute("ViewBooking", booking.Id, ResponseUtil<RoomBookingHistoryDTO>.Ok(booking));
        }
    }
}
