using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using Biz.BrightOnion.Ordering.Domain.Seedwork;
using Biz.BrightOnion.Ordering.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.Infrastructure
{
  public class OrderingContext : DbContext, IUnitOfWork
  {
    public DbSet<Order> Orders { get; set; }

    public OrderingContext() : base() { }
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
      // Dispatch Domain Events collection. 
      // Choices:
      // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
      // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
      // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
      // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
      // await mediator.DispatchDomainEventsAsync(this);

      // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
      // performed through the DbContext will be committed
      var result = await base.SaveChangesAsync();

      return true;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
      modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
    }
  }
}
