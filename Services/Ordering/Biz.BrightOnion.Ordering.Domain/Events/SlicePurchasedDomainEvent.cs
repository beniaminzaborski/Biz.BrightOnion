using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Domain.Events
{
  public class SlicePurchasedDomainEvent : INotification
  {
    public long OrderId { get; private set; }
    public DateTime Day { get; private set; }
    public long RoomId { get; private set; }
    public long PurchaserId { get; private set; }
    public int Quantity { get; private set; }

    public SlicePurchasedDomainEvent(
      long orderId,
      DateTime day,
      long roomId,
      long purchaserId,
      int quantity)
    {
      OrderId = orderId;
      Day = day;
      RoomId = roomId;
      PurchaserId = purchaserId;
      Quantity = quantity;
    }
  }
}
