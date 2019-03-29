using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Queries
{
  public class OrderQueries : IOrderQueries
  {
    private string connectionString = string.Empty;

    public OrderQueries(string connectionString)
    {
      this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<Order> GetOrderInRoomForDayAsync(long roomId, DateTime day)
    {
      Order order = null;

      using (var connection = new SqlConnection(connectionString))
      {
        connection.Open();

        order = await connection.QueryFirstOrDefaultAsync<Order>(@"
          SELECT 
	          o.Id as OrderId, o.Day, o.RoomId, o.TotalPizzas, o.FreeSlicesToGrab
          FROM Orders o
          WHERE o.RoomId = @roomId and o.Day = @day", new { roomId, day });

        if (order != null)
        {
          var orderItems = await connection.QueryAsync<OrderItem>(@"
          SELECT 
            i.Id as OrderItemId, i.PurchaserId, p.Email AS PurchaserEmail, i.Quantity 
          FROM Orders o
          INNER JOIN OrderItems i ON i.OrderId = o.Id
          LEFT JOIN Purchasers p ON p.Id = i.PurchaserId
          WHERE o.RoomId = @roomId and o.Day = @day", new { roomId, day });

          order.OrderItems = orderItems?.ToList();
        }
      }

      return order;
    }
  }
}
