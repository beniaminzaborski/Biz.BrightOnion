using Biz.BrightOnion.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Services
{
  public interface IIntegrationEventLogService
  {
    Task SaveEventAsync(IntegrationEvent integrationEvent);

    Task MarkEventAsPublishedAsync(Guid eventId);
  }
}
