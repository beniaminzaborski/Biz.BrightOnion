using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Domain.Events
{
  public class NewOrderCreatedDomainEvent : INotification
  {
    public long OrderId { get; private set; }
    public DateTime Day { get; private set; }
    public long RoomId { get; private set; }
    public string RoomName { get; private set; }
    public long PurchaserId { get; private set; }

    public NewOrderCreatedDomainEvent(
      long orderId,
      DateTime day,
      long roomId,
      string roomName,
      long purchaserId)
    {
      OrderId = orderId;
      Day = day;
      RoomId = roomId;
      RoomName = roomName;
      PurchaserId = purchaserId;
    }
  }
}
