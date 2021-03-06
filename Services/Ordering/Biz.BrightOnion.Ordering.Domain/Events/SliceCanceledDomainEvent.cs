﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Domain.Events
{
  public class SliceCanceledDomainEvent : INotification
  {
    public long OrderId { get; private set; }
    public long RoomId { get; private set; }
    public DateTime Day { get; private set; }
    public long PurchaserId { get; private set; }

    public SliceCanceledDomainEvent(
      long orderId,
      long roomId,
      DateTime day,
      long purchaserId)
    {
      OrderId = orderId;
      RoomId = roomId;
      Day = day;
      PurchaserId = purchaserId;
    }
  }
}
