using Biz.BrightOnion.Catalog.API.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Infrastructure.Repositories
{
  public interface IRepository<TEntity>
    where TEntity : Entity
  {
    IQueryable<TEntity> GetAll();

    Task<TEntity> GetById(long id);

    Task<long> Create(TEntity entity);

    Task Update(long id, TEntity entity);

    Task Delete(long id);
  }
}
