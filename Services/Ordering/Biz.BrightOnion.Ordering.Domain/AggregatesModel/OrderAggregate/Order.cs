using Biz.BrightOnion.Ordering.Domain.Seedwork;
using System;

namespace Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate
{
  public class Order : Entity, IAggregateRoot
  {
    public long RoomId { get; private set; }
    public long PurchaserId { get; private set; }
    public DateTime Day { get; private set; }
    public int Quantity { get; private set; }

    public Order(
      long roomId,
      long purchaserId,
      DateTime day,
      int quantity)
    {
      this.RoomId = roomId;
      this.PurchaserId = purchaserId;
      this.Day = day;
      this.Quantity = quantity;
    }
  }
}
