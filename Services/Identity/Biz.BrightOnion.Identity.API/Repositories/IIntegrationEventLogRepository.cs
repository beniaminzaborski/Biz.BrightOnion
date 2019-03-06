using Biz.BrightOnion.Identity.API.Entities;
using Biz.BrightOnion.Identity.API.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Repositories
{
  public interface IIntegrationEventLogRepository : IRepository<IntegrationEventLog>
  {
    Task<IntegrationEventLog> GetByEventIdAsync(Guid eventId);

    Task<bool> CheckIfExistsByEventIdAsync(Guid eventId);
  }
}
