using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biz.BrightOnion.EventBus.Events;
using Biz.BrightOnion.Identity.API.Entities;
using Biz.BrightOnion.Identity.API.Repositories;
using Newtonsoft.Json;
using NGuard;

namespace Biz.BrightOnion.Identity.API.Services
{
  public class IntegrationEventLogService : IIntegrationEventLogService
  {
    private readonly IIntegrationEventLogRepository repository;

    public IntegrationEventLogService(IIntegrationEventLogRepository repository)
    {
      this.repository = repository;
    }

    public async Task SaveEventAsync(IntegrationEvent integrationEvent)
    {
      Guard.Requires(integrationEvent, nameof(integrationEvent)).IsNotNull();

      if (repository.GetAll().Any(e => e.EventId == integrationEvent.EventId))
        throw new Exception($"Internal error - event id: {integrationEvent.EventId} exists in event log");

      await repository.CreateAsync(
        new IntegrationEventLog
        {
          State = IntegrationEventState.ReadyToPublish,
          EventId = integrationEvent.EventId,
          EventCreationDate = integrationEvent.EventCreationDate,
          EventType = integrationEvent.GetType().Name,
          EventBody = JsonConvert.SerializeObject(integrationEvent)
        }
      );
    }

    public async Task MarkEventAsPublishedAsync(Guid eventId)
    {
      Guard.Requires(eventId, nameof(eventId)).IsNotEqualTo(Guid.Empty);

      var integrationEvent = repository.GetAll().First(e => e.EventId == eventId);

      if (integrationEvent.State == IntegrationEventState.ReadyToPublish)
      {
        integrationEvent.State = IntegrationEventState.Published;

        await repository.UpdateAsync(integrationEvent.Id, integrationEvent);
      }
    }
  }
}
