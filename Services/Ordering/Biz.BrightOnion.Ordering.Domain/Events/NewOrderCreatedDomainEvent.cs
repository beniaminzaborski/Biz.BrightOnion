using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Domain.Events
{
  public class NewOrderCreatedDomainEvent : INotification
  {
    public DateTime Day { get; private set; }
    public string RoomName { get; private set; }
    public long PurchaserId { get; private set; }

    public NewOrderCreatedDomainEvent(
      DateTime day,
      string roomName,
      long purchaserId)
    {
      this.Day = day;
      this.RoomName = roomName;
      this.PurchaserId = purchaserId;
    }
  }
}
