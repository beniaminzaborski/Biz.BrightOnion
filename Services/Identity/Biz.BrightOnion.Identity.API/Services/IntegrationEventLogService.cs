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

      if (repository.GetAll().Any(e => e.EventId == integrationEvent.EventId))
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

    public async Task MarkEventAsPublishedAsync(IntegrationEvent integrationEvent)
    {
      Guard.Requires(integrationEvent, nameof(integrationEvent)).IsNotNull();

      var integrationEventLog = await repository.GetByEventIdAsync(integrationEvent.EventId);
      if(integrationEventLog == null)
        throw new Exception($"Internal error - event id: {integrationEvent.EventId} does not exist in event log");

      if (integrationEventLog.State == IntegrationEventState.ReadyToPublish)
      {
        integrationEventLog.State = IntegrationEventState.Published;
        await repository.UpdateAsync(integrationEventLog.Id, integrationEventLog);
      }
    }

    public async Task<IEnumerable<IntegrationEventLog>> GetUnpublishedEventsAsync(int count)
    {
      return await dbContext.Set<IntegrationEventLog>()
        .Where(e => e.State == IntegrationEventState.ReadyToPublish)
        .OrderBy(e => e.EventCreationDate)
        .Take(10)
        .ToListAsync();
    }

    //public async Task<bool> CheckIsEventPublished(IntegrationEvent integrationEvent)
    //{
    //  return await dbContext.Set<IntegrationEventLog>()
    //    .Where(e => e.EventId == integrationEvent.EventId)
    //    .Select(e => e.State == IntegrationEventState.Published)
    //    .FirstOrDefaultAsync();
    //}
  }
}
