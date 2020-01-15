using Biz.BrightOnion.Rooms.API.Entities;
using Biz.BrightOnion.Rooms.API.Infrastructure.Repositories;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Rooms.API.Repositories
{
  public class RoomRepository : Repository<Room>, IRoomRepository
  {
    public RoomRepository(ISession session) : base(session) { }

    public async Task<Room> GetByNameAsync(string name)
    {
      return await session.Query<Room>().Where(r => r.Name == name).FirstOrDefaultAsync();
    }
  }
}
