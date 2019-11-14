using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.Ordering.API.Events;
using Biz.BrightOnion.Ordering.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.DomainEventHandlers
{
  public class PublishIntegrationEventWhenOrderApprovedDomainEventHandler : INotificationHandler<OrderApprovedDomainEvent>
  {
    private readonly IEventBus eventBus;

    public PublishIntegrationEventWhenOrderApprovedDomainEventHandler(IEventBus eventBus)
    {
      this.eventBus = eventBus;
    }

    public async Task Handle(OrderApprovedDomainEvent notification, CancellationToken cancellationToken)
    {
      var orderStatusChangedEvent = new OrderStatusChangedEvent(notification.OrderId, notification.RoomId, notification.Day);
      eventBus.Publish(orderStatusChangedEvent);
    }
  }
}
