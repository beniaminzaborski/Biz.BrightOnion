using Biz.BrightOnion.Ordering.Domain.Seedwork;
using System;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate
{
  public interface IOrderRepository : IRepository<Order>
  {
    Order Add(Order order);

    void Update(Order order);

    Task<Order> GetAsync(int orderId);

    Task<Order> GetByDayEagerAsync(DateTime day);
  }
}
