using Biz.BrightOnion.EventBus.Events;
using System;

namespace Biz.BrightOnion.Ordering.SignalrHub.Events
{
  public class OrderStatusChangedEvent : IntegrationEvent
  {
    public long OrderId { get; set; }
    public long RoomId { get; set; }
    public DateTime Day { get; set; }

    public OrderStatusChangedEvent(long orderId, long roomId, DateTime day)
    {
      OrderId = orderId;
      RoomId = roomId;
      Day = day;
    }
  }
}
