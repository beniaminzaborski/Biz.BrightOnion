using Biz.BrightOnion.Ordering.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate
{
  public class Order : Entity, IAggregateRoot
  {
    public long RoomId { get; private set; }
    public int SlicesPerPizza { get; private set; }
    public DateTime Day { get; private set; }
    public int TotalPizzas { get; private set; }
    public int FreeSlicesToGrab { get; private set; }
    private readonly List<OrderItem> orderItems;
    public IReadOnlyCollection<OrderItem> OrderItems => orderItems;
    public bool IsApproved { get; private set; }

    protected Order()
    {
      orderItems = new List<OrderItem>();
    }

    public Order(
      long roomId,
      int slicesPerPizza,
      DateTime day) : this()
    {
      this.RoomId = roomId;
      this.SlicesPerPizza = slicesPerPizza;
      this.Day = day;
    }

    public void AddOrderItem(long purchaserId, int quantity)
    {
      if (quantity <= 0)
        return;

      var existingOrderForPurchaser = orderItems.Where(o => o.PurchaserId == purchaserId)
        .SingleOrDefault();

      if (existingOrderForPurchaser != null)
      {
        if (quantity > 0)
          existingOrderForPurchaser.SetQuantity(quantity);
        else
          orderItems.Remove(existingOrderForPurchaser);
      }
      else
      {
        var orderItem = new OrderItem(purchaserId, quantity);
        orderItems.Add(orderItem);
      }

      CalculateTotalPizzas();
      CalculateFreeSlicesToGrab();
    }

    public void SetSlicesPerPizza(int slicesPerPizza)
    {
      this.SlicesPerPizza = slicesPerPizza;
    }

    public void RemoveOrderItem(long purchaserId)
    {
      var existingOrderForPurchaser = orderItems.Where(o => o.PurchaserId == purchaserId)
        .SingleOrDefault();

      orderItems.Remove(existingOrderForPurchaser);

      CalculateTotalPizzas();
      CalculateFreeSlicesToGrab();
    }

    public void Approve()
    {
      IsApproved = true;
    }

    private int GetTotalSlices()
    {
      return orderItems.Sum(s => s.Quantity);
    }

    private void CalculateTotalPizzas()
    {
      int pizzas = GetTotalSlices() / SlicesPerPizza;
      if (GetTotalSlices() % SlicesPerPizza > 0)
        pizzas = pizzas + 1;
      this.TotalPizzas = pizzas;
    }

    private void CalculateFreeSlicesToGrab()
    {
      int totalPizzasSlices = TotalPizzas * SlicesPerPizza;
      this.FreeSlicesToGrab = totalPizzasSlices - GetTotalSlices();
    }
  }
}
