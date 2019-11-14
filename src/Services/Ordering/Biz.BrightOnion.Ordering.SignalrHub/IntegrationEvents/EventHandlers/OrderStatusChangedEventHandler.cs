using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.Ordering.SignalrHub.Events;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.SignalrHub.IntegrationEvents.EventHandlers
{
  public class OrderStatusChangedEventHandler : IIntegrationEventHandler<OrderStatusChangedEvent>
  {
    private readonly IHubContext<NotificationsHub> hubContext;

    public OrderStatusChangedEventHandler(IHubContext<NotificationsHub> hubContext)
    {
      this.hubContext = hubContext;
    }

    public async Task Handle(OrderStatusChangedEvent @event)
    {
      await hubContext.Clients
        // .Group(@event.BuyerName)
        .All
        .SendAsync("UpdatedOrderStatus", new { OrderId = @event.OrderId, RoomId = @event.RoomId, Day = @event.Day });
    }
  }
}
