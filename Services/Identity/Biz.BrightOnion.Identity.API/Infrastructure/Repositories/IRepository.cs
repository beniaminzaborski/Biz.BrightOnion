using Biz.BrightOnion.Identity.API.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Infrastructure.Repositories
{
  public interface IUserRepository<TEntity>
    where TEntity: Entity
  {
    IQueryable<TEntity> GetAll();

    Task<TEntity> GetById(long id);

    Task Create(TEntity entity);

    Task Update(long id, TEntity entity);

    Task Delete(long id);
  }
}
