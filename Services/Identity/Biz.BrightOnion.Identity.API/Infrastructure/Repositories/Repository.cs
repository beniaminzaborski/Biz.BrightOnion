using Biz.BrightOnion.Identity.API.Data;
using Biz.BrightOnion.Identity.API.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using NGuard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Infrastructure.Repositories
{
  public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
  {
    protected readonly ApplicationContext dbContext;
    public Repository(ApplicationContext dbContext)
    {
      this.dbContext = dbContext;
    }

    public IQueryable<TEntity> GetAll()
    {
      return dbContext.Set<TEntity>().AsNoTracking();
    }

    public async Task<TEntity> GetByIdAsync(long id)
    {
      return await dbContext.Set<TEntity>()
                  .AsNoTracking()
                  .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task CreateAsync(TEntity entity)
    {
      Guard.Requires(entity, nameof(entity)).IsNotNull();

      await dbContext.Set<TEntity>().AddAsync(entity);
    }

    public async Task UpdateAsync(long id, TEntity entity)
    {
      Guard.Requires(entity, nameof(entity)).IsNotNull();

      dbContext.Set<TEntity>().Update(entity);
    }

    public async Task DeleteAsync(long id)
    {
      var entity = await GetByIdAsync(id);
      dbContext.Set<TEntity>().Remove(entity);
    }
  }
}
