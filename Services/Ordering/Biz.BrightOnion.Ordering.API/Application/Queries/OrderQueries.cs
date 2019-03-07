using System;
using System.Collections.Generic;
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

    public Task<IEnumerable<Order>> GetOrdersInRoomAsync(long roomId)
    {
      throw new NotImplementedException();
    }
  }
}
