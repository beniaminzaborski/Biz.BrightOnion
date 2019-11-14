using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using Biz.BrightOnion.Ordering.Domain.AggregatesModel.PurchaserAggregate;
using Biz.BrightOnion.Ordering.Domain.Seedwork;
using Biz.BrightOnion.Ordering.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.Infrastructure
{
  public class OrderingContext : DbContext, IUnitOfWork
  {
    private readonly IMediator mediator;
    private IDbContextTransaction currentTransaction;

    public IDbContextTransaction GetCurrentTransaction => currentTransaction;
    public bool HasActiveTransaction => currentTransaction != null;

    public DbSet<Order> Orders { get; set; }
    public DbSet<Purchaser> Purchasers { get; set; }

    //public OrderingContext() : base() { }
    public OrderingContext(
      DbContextOptions<OrderingContext> options, 
      IMediator mediator) : base(options)
    {
      this.mediator = mediator;
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
      // Dispatch Domain Events collection. 
      // Choices:
      // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
      // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
      // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
      // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
      await mediator.DispatchDomainEventsAsync(this);

      // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
      // performed through the DbContext will be committed
      var result = await base.SaveChangesAsync();

      return true;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
      if (currentTransaction != null) return null;

      currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

      return currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
      if (transaction == null) throw new ArgumentNullException(nameof(transaction));
      if (transaction != currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

      try
      {
        await SaveChangesAsync();
        transaction.Commit();
      }
      catch
      {
        RollbackTransaction();
        throw;
      }
      finally
      {
        if (currentTransaction != null)
        {
          currentTransaction.Dispose();
          currentTransaction = null;
        }
      }
    }

    public void RollbackTransaction()
    {
      try
      {
        currentTransaction?.Rollback();
      }
      finally
      {
        if (currentTransaction != null)
        {
          currentTransaction.Dispose();
          currentTransaction = null;
        }
      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
      modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());

      modelBuilder.ApplyConfiguration(new PurchaserEntityTypeConfiguration());
    }
  }
}
