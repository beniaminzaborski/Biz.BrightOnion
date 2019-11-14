using Biz.BrightOnion.Catalog.API.Entities;
using Biz.BrightOnion.Catalog.API.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Repositories
{
  public interface IRoomRepository : IRepository<Room>
  {
    Task<Room> GetByNameAsync(string name);
  }
}
