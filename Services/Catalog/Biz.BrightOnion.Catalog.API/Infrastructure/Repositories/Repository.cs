using BarsGroup.CodeGuard;
using Biz.BrightOnion.Catalog.API.Infrastructure.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Infrastructure.Repositories
{
  public class Repository<TEntity> : IRepository<TEntity>
      where TEntity : Entity
  {
    protected readonly ISession session;
    public Repository(ISession session)
    {
      this.session = session;
    }

    public IQueryable<TEntity> GetAll()
    {
      return session.Query<TEntity>();
    }

    public async Task<TEntity> GetById(long id)
    {
      return await session.Query<TEntity>()
                  .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<long> Create(TEntity entity)
    {
      Guard.That(entity).IsNotNull();

      return await session.SaveAsync(entity).ContinueWith(t => (long)t.Result);
    }

    public async Task Update(long id, TEntity entity)
    {
      Guard.That(entity).IsNotNull();

      await session.SaveOrUpdateAsync(entity);
    }

    public async Task Delete(long id)
    {
      var entity = await session.LoadAsync<TEntity>(id);
      await session.DeleteAsync(entity);
    }
  }
}
