using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Biz.BrightOnion.Ordering.UnitTests.AggregatesModel.OrderAggregate
{
  public class Order_GetTotalSlices_Tests
  {
    [Fact]
    public void EmptyOrderItems_ShouldReturn0()
    {
      // Arrange
      var order = new Order(1, new DateTime(2019, 3, 12));

      // Act
      int slices = order.GetTotalSlices();

      // Assert
      Assert.Equal(0, slices);
    }

    [Fact]
    public void OneOrderItemWith5Slices_ShouldReturn5()
    {
      // Arrange
      var order = new Order(1, new DateTime(2019, 3, 12));
      order.AddOrderItem(1, 5);

      // Act
      int slices = order.GetTotalSlices();

      // Assert
      Assert.Equal(5, slices);
    }

    [Fact]
    public void TwoOrderItemsWith5And3Slices_ShouldReturn8()
    {
      // Arrange
      var order = new Order(1, new DateTime(2019, 3, 12));
      order.AddOrderItem(1, 5);
      order.AddOrderItem(2, 3);

      // Act
      int slices = order.GetTotalSlices();

      // Assert
      Assert.Equal(8, slices);
    }

    [Fact]
    public void UpdateOrderItemWith3Slices_ShouldReturn3()
    {
      // Arrange
      var order = new Order(1, new DateTime(2019, 3, 12));
      order.AddOrderItem(1, 5);
      order.AddOrderItem(1, 3);

      // Act
      int slices = order.GetTotalSlices();

      // Assert
      Assert.Equal(3, slices);
    }

    [Fact]
    public void AddAndRemoveOrderItem_ShouldReturn0()
    {
      // Arrange
      var order = new Order(1, new DateTime(2019, 3, 12));
      order.AddOrderItem(1, 5);
      order.RemoveOrderItem(1);

      // Act
      int slices = order.GetTotalSlices();

      // Assert
      Assert.Equal(0, slices);
    }
  }
}
