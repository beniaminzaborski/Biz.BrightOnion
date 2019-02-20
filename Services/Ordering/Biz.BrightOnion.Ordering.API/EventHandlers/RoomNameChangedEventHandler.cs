using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.Ordering.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.EventHandlers
{
  public class RoomNameChangedEventHandler : IIntegrationEventHandler<RoomNameChangedEvent>
  {
    public Task Handle(RoomNameChangedEvent @event)
    {
      Console.WriteLine("Handle RoomNameChangedEvent: {0}", @event.Name);
      return Task.Delay(1000);
    }
  }
}
