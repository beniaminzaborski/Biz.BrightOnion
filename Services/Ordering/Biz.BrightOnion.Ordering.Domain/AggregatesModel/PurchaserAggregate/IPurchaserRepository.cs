using Biz.BrightOnion.Ordering.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.Domain.AggregatesModel.PurchaserAggregate
{
  public interface IPurchaserRepository : IRepository<Purchaser>
  {
    Task<Purchaser> Get(long id);

    Task<bool> CheckIfExistsAsync(long id);

    Purchaser Add(Purchaser purchaser);

    void Update(Purchaser purchaser);
  }
}
