using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Queries
{
  public interface IOrderQueries
  {
    Task<Order> GetOrderInRoomForDayAsync(long roomId, DateTime day);
  }
}
