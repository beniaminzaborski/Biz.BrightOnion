using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.Ordering.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.EventHandlers
{
  public class RoomDeletedEventHandler : IIntegrationEventHandler<RoomDeletedEvent>
  {
    public Task Handle(RoomDeletedEvent @event)
    {
      Console.WriteLine("Handle RoomDeletedEvent: {0}", @event.Id);
      return Task.Delay(1000);
    }
  }
}
