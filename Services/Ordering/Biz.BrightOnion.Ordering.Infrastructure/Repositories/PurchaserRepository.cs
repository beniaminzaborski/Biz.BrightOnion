using Biz.BrightOnion.Ordering.Domain.AggregatesModel.PurchaserAggregate;
using Biz.BrightOnion.Ordering.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.Infrastructure.Repositories
{
  public class PurchaserRepository : IPurchaserRepository
  {
    private readonly OrderingContext context;

    public IUnitOfWork UnitOfWork
    {
      get
      {
        return context;
      }
    }

    public async Task<Purchaser> Get(long id)
    {
      return await context.Purchasers.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> CheckIfExistsAsync(long id)
    {
      return await context.Purchasers.AnyAsync(p => p.Id == id);
    }

    public PurchaserRepository(OrderingContext context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Purchaser Add(Purchaser purchaser)
    {
      return context.Purchasers.Add(purchaser).Entity;
    }

    public void Update(Purchaser purchaser)
    {
      context.Entry(purchaser).State = EntityState.Modified;
    }
  }
}
