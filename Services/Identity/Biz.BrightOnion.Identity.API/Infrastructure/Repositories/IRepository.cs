using Biz.BrightOnion.Identity.API.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Infrastructure.Repositories
{
  public interface IRepository<TEntity>
    where TEntity: Entity
  {
    IQueryable<TEntity> GetAll();

    Task<TEntity> GetByIdAsync(long id);

    Task CreateAsync(TEntity entity);

    Task UpdateAsync(long id, TEntity entity);

    Task DeleteAsync(long id);
  }
}
