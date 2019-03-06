using Biz.BrightOnion.Identity.API.Infrastructure.Entities;
using Biz.BrightOnion.Identity.API.Infrastructure.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Infrastructure.Repositories
{
  public interface IRepository<TEntity>
    where TEntity: Entity
  {
    IEnumerable<TEntity> Find(ISpecification<TEntity> spec = null);

    Task<TEntity> GetByIdAsync(long id);

    Task CreateAsync(TEntity entity);

    void Update(long id, TEntity entity);

    Task DeleteAsync(long id);
  }
}
