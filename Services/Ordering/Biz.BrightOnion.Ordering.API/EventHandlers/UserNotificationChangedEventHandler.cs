using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.Ordering.API.Application.Commands;
using Biz.BrightOnion.Ordering.API.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.EventHandlers
{
  public class UserNotificationChangedEventHandler : IIntegrationEventHandler<UserNotificationChangedEvent>
  {
    private readonly IMediator mediator;

    public UserNotificationChangedEventHandler(IMediator mediator)
    {
      this.mediator = mediator;
    }

    public async Task Handle(UserNotificationChangedEvent @event)
    {
      Console.WriteLine("Handle UserNotificationChangedEvent: {0}", @event.NotificationEnabled);

      var changePurchaserNotificationCommand = new ChangePurchaserNotificationCommand(@event.Id, @event.NotificationEnabled);

      await mediator.Send(changePurchaserNotificationCommand);
    }
  }
}
