using Biz.BrightOnion.EventBus.Events;
using Biz.BrightOnion.Identity.API.Entities;
using Biz.BrightOnion.Identity.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Services
{
  public interface IIntegrationEventLogService
  {
    Task SaveEventAsync(IntegrationEvent integrationEvent);

    Task MarkEventAsPublishedAsync(IntegrationEvent integrationEvent);

    Task<IEnumerable<IntegrationEventLog>> GetUnpublishedEventsAsync(int count);

    // Task<bool> CheckIsEventPublished(IntegrationEvent integrationEvent);
  }
}
