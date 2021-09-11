using Core.DI;
using Core.Models;
using Infrastructure.Documents;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{   

    public class RoomStateMachine
    {
        RoomStates _state;
        StateMachine<RoomStates, Trigger> _machine;
        enum Trigger { BookRoom, CheckIn };

        public RoomStateMachine(IClientService clientService, IRoomService roomService)
        {
            //var stateMachine = new StateMachine<RoomStates, Trigger>(RoomStates.Free);

            _state = RoomStates.Free;
            _machine = new StateMachine<RoomStates, Trigger>(RoomStates.Free);

            _machine.Configure(RoomStates.Free)
                    .Permit(Trigger.BookRoom, RoomStates.Reserved);

            _machine.Configure(RoomStates.Reserved)
                .OnEntry(t => clientService.UpdateClientStateAsync("", ClientStates.BookedRoom))
                    .Permit(Trigger.BookRoom, RoomStates.Reserved);

            //_machine.Configure(ClientStates.OFF)
            //        .PermitIf(Trigger.TOGGLE, ClientStates.ON, () => IsLightNeeded(),
            //           "Toggle allowed")
            //        .PermitReentryIf(Trigger.TOGGLE, () => !IsLightNeeded(),
            //           "Toggle not allowed");
        }
    }
}
