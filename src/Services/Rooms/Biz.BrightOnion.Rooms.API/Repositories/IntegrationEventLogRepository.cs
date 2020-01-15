using Biz.BrightOnion.Rooms.API.Entities;
using Biz.BrightOnion.Rooms.API.Infrastructure.Repositories;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Rooms.API.Repositories
{
  public class IntegrationEventLogRepository : Repository<IntegrationEventLog>, IIntegrationEventLogRepository
  {
    public IntegrationEventLogRepository(ISession session) : base(session) { }
  }
}
