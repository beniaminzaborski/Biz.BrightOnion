using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Biz.BrightOnion.Ordering.UnitTests.AggregatesModel.OrderAggregate
{
  public class Order_GetTotalPizzas_Tests
  {
    [Fact]
    public void TotalQunatityLowerThan8_ShouldReturn1()
    {
      // Arrange
      var order = new Order(1, 8, new DateTime(2019, 3, 12));
      order.AddOrderItem(1, 2);
      order.AddOrderItem(2, 1);

      // Act
      int pizzas = order.TotalPizzas;

      // Assert
      Assert.Equal(1, pizzas);
    }

    [Fact]
    public void TotalQunatityEquals8_ShouldReturn1()
    {
      // Arrange
      var order = new Order(1, 8, new DateTime(2019, 3, 12));
      order.AddOrderItem(1, 5);
      order.AddOrderItem(2, 3);

      // Act
      int pizzas = order.TotalPizzas;

      // Assert
      Assert.Equal(1, pizzas);
    }

    [Fact]
    public void TotalQunatityGreaterThan8AndLowerThan16_ShouldReturn2()
    {
      // Arrange
      var order = new Order(1, 8, new DateTime(2019, 3, 12));
      order.AddOrderItem(1, 5);
      order.AddOrderItem(2, 3);
      order.AddOrderItem(3, 2);

      // Act
      int pizzas = order.TotalPizzas;

      // Assert
      Assert.Equal(2, pizzas);
    }

    [Fact]
    public void TotalQunatityEquals16_ShouldReturn2()
    {
      // Arrange
      var order = new Order(1, 8, new DateTime(2019, 3, 12));
      order.AddOrderItem(1, 5);
      order.AddOrderItem(2, 3);
      order.AddOrderItem(3, 8);

      // Act
      int pizzas = order.TotalPizzas;

      // Assert
      Assert.Equal(2, pizzas);
    }
  }
}
