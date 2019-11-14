using Biz.BrightOnion.Catalog.API.Entities;
using Biz.BrightOnion.Catalog.API.Infrastructure.Repositories;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Repositories
{
  public class IntegrationEventLogRepository : Repository<IntegrationEventLog>, IIntegrationEventLogRepository
  {
    public IntegrationEventLogRepository(ISession session) : base(session) { }
  }
}
