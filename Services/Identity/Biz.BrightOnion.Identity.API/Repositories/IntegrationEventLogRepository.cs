using Biz.BrightOnion.Identity.API.Data;
using Biz.BrightOnion.Identity.API.Entities;
using Biz.BrightOnion.Identity.API.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Repositories
{
  public class IntegrationEventLogRepository : Repository<IntegrationEventLog>, IIntegrationEventLogRepository
  {
    public IntegrationEventLogRepository(ApplicationContext dbContext) : base(dbContext) { }

    public async Task<IntegrationEventLog> GetByEventIdAsync(Guid eventId)
    {
      return await dbContext.Set<IntegrationEventLog>()/*IntegrationEventLogs*/.Where(e => e.EventId == eventId).FirstOrDefaultAsync();
    }
  }
}
