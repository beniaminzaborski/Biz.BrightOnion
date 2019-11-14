using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Domain.Events
{
  public class OrderApprovedDomainEvent : INotification
  {
    public long OrderId { get; private set; }
    public long RoomId { get; private set; }
    public DateTime Day { get; private set; }

    public OrderApprovedDomainEvent(
      long orderId,
      long roomId,
      DateTime day)
    {
      OrderId = orderId;
      RoomId = roomId;
      Day = day;
    }
  }
}
