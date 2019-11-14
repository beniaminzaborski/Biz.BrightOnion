using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Biz.BrightOnion.Ordering.UnitTests.AggregatesModel.OrderAggregate
{
  public class Order_GetFreeSlicesToGrab_Tests
  {
    [Fact]
    public void EmptyOrderItems_ShouldReturn0()
    {
      // Arrange
      var order = new Order(1, 8, new DateTime(2019, 3, 12));

      // Act
      int freeSlicesToGrab = order.FreeSlicesToGrab;

      // Assert
      Assert.Equal(0, freeSlicesToGrab);
    }

    [Fact]
    public void TotalSlices1_ShouldReturn7()
    {
      // Arrange
      var order = new Order(1, 8, new DateTime(2019, 3, 12));
      order.AddOrderItem(1, 1);

      // Act
      int freeSlicesToGrab = order.FreeSlicesToGrab;

      // Assert
      Assert.Equal(7, freeSlicesToGrab);
    }

    [Fact]
    public void TotalSlices8_ShouldReturn0()
    {
      // Arrange
      var order = new Order(1, 8, new DateTime(2019, 3, 12));
      order.AddOrderItem(1, 8);

      // Act
      int freeSlicesToGrab = order.FreeSlicesToGrab;

      // Assert
      Assert.Equal(0, freeSlicesToGrab);
    }

    [Fact]
    public void TotalSlice11_ShouldReturn5()
    {
      // Arrange
      var order = new Order(1, 8, new DateTime(2019, 3, 12));
      order.AddOrderItem(1, 8);
      order.AddOrderItem(2, 3);

      // Act
      int freeSlicesToGrab = order.FreeSlicesToGrab;

      // Assert
      Assert.Equal(5, freeSlicesToGrab);
    }
  }
}
