﻿using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
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

    public async Task<Order> GetAsync(int orderId)
    {
      return await context.Orders.FindAsync(orderId);
    }

    public void Update(Order order)
    {
      context.Entry(order).State = EntityState.Modified;
    }
  }
}
