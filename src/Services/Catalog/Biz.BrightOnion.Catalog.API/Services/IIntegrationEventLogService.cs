using Biz.BrightOnion.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Services
{
  public interface IIntegrationEventLogService
  {
    Task<long> SaveEventAsync(IntegrationEvent integrationEvent);

    Task MarkEventAsPublishedAsync(Guid eventId);
  }
}
