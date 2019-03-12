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

    public async Task<IEnumerable<Order>> GetOrdersInRoomAsync(long roomId)
    {
      using (var connection = new SqlConnection(connectionString))
      {
        connection.Open();

        return await connection.QueryAsync<Order>(@"
          SELECT 
            o.Id as OrderId, o.Day, o.RoomId, i.PurchaserId, i.Quantity 
          FROM Orders o
          INNER JOIN OrderItems i ON i.OrderId = o.Id
          WHERE o.RoomId = @roomId", new { roomId });
      }
    }
  }
}
