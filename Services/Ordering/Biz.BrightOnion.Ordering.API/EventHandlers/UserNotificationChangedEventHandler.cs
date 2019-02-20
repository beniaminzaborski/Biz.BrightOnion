using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.Ordering.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.EventHandlers
{
  public class UserNotificationChangedEventHandler : IIntegrationEventHandler<UserNotificationChangedEvent>
  {
    public Task Handle(UserNotificationChangedEvent @event)
    {
      Console.WriteLine("Handle UserNotificationChangedEvent: {0}", @event.NotificationEnabled);
      return Task.Delay(1000);
    }
  }
}
