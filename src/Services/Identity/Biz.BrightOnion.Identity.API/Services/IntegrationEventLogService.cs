using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biz.BrightOnion.EventBus.Events;
using Biz.BrightOnion.Identity.API.Data;
using Biz.BrightOnion.Identity.API.Entities;
using Biz.BrightOnion.Identity.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NGuard;

namespace Biz.BrightOnion.Identity.API.Services
{
  public class IntegrationEventLogService : IIntegrationEventLogService
  {
    private readonly ApplicationContext dbContext;
    private readonly IIntegrationEventLogRepository repository;

    public IntegrationEventLogService(
      ApplicationContext dbContext,
      IIntegrationEventLogRepository repository)
    {
      this.dbContext = dbContext;
      this.repository = repository;
    }

    public async Task SaveEventAsync(IntegrationEvent integrationEvent)
    {
      Guard.Requires(integrationEvent, nameof(integrationEvent)).IsNotNull();

      if (await repository.CheckIfExistsByEventIdAsync(integrationEvent.EventId))
        throw new Exception($"Internal error - event id: {integrationEvent.EventId} exists in event log");

      await repository.CreateAsync(
        new IntegrationEventLog
        {
          State = IntegrationEventState.ReadyToPublish,
          EventId = integrationEvent.EventId,
          EventCreationDate = integrationEvent.EventCreationDate,
          EventType = integrationEvent.GetType().FullName,
          EventBody = JsonConvert.SerializeObject(integrationEvent)
        }
      );
    }
  }
}
