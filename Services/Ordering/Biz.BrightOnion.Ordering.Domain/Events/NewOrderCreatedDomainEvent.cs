using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Domain.Events
{
  public class NewOrderCreatedDomainEvent : INotification
  {
    public DateTime Day { get; private set; }
    public long RoomId { get; set; }
    public long PurchaserId { get; private set; }

    public NewOrderCreatedDomainEvent(
      DateTime day,
      long roomId,
      long purchaserId)
    {
      this.Day = day;
      this.RoomId = roomId;
      this.PurchaserId = purchaserId;
    }
  }
}
