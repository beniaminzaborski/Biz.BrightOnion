using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using Biz.BrightOnion.Ordering.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.Infrastructure.Repositories
{
  public class OrderRepository : IOrderRepository
  {
    private readonly OrderingContext context;

    public IUnitOfWork UnitOfWork
    {
      get
      {
        return context;
      }
    }

    public OrderRepository(OrderingContext context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Order Add(Order order)
    {
      return context.Orders.Add(order).Entity;
    }

    public async Task<Order> GetAsync(long orderId)
    {
      return await context.Orders.FindAsync(orderId);
    }

    public async Task<Order> GetByDayAndRoomEagerAsync(DateTime day, long roomId)
    {
      return await context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Day == day.Date && o.RoomId == roomId);
    }

    public void Update(Order order)
    {
      context.Entry(order).State = EntityState.Modified;
    }
  }
}
