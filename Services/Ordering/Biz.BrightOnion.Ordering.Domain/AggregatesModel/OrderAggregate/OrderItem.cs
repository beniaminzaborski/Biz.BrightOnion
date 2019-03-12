using Biz.BrightOnion.Ordering.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate
{
  public class OrderItem : Entity
  {
    public long PurchaserId { get; private set; }
    public int Quantity { get; private set; }

    protected OrderItem() { }

    public OrderItem(
      long purchaserId,
      int quantity)
    {
      this.PurchaserId = purchaserId;
      this.Quantity = quantity;
    }

    public void SetQuantity(int quantity)
    {
      this.Quantity = quantity;
    }
  }
}
