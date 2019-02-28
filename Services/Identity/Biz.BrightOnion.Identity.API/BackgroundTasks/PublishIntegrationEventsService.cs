using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.EventBus.Events;
using Biz.BrightOnion.Identity.API.Data;
using Biz.BrightOnion.Identity.API.Entities;
using Biz.BrightOnion.Identity.API.Events;
using Biz.BrightOnion.Identity.API.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.BackgroundTasks
{
  public class PublishIntegrationEventsService : BackgroundService
  {
    private readonly ILogger<PublishIntegrationEventsService> logger;
    private readonly ApplicationContext dbContext;
    private readonly IIntegrationEventLogService integrationEventLogService;
    private readonly IEventBus eventBus;

    public PublishIntegrationEventsService(
      ILogger<PublishIntegrationEventsService> logger,
      ApplicationContext dbContext,
      IIntegrationEventLogService integrationEventLogService,
      IEventBus eventBus)
    {
      this.logger = logger;
      this.dbContext = dbContext;
      this.integrationEventLogService = integrationEventLogService;
      this.eventBus = eventBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      logger.LogDebug($"PublishIntegrationEventsService is starting.");

      stoppingToken.Register(() =>
        logger.LogDebug($"PublishIntegrationEventsService background task is stopping."));

      while (!stoppingToken.IsCancellationRequested)
      {
        logger.LogDebug($"PublishIntegrationEventsService task doing background work.");
        
        // This eShopOnContainers method is querying a database table
        // and publishing events into the Event Bus (RabbitMS / ServiceBus)
        await PublishAllUnpublishedIntegrationEvents();

        await Task.Delay(10000, stoppingToken);
      }

      logger.LogDebug($"PublishIntegrationEventsService background task is stopping.");
    }

    private async Task PublishAllUnpublishedIntegrationEvents()
    {
      using (var transaction = dbContext.Database.BeginTransaction())
      {
        IEnumerable<IntegrationEventLog> eventsToPublish = await integrationEventLogService.GetUnpublishedEventsAsync(10); // TODO: Move count to configuration
        foreach (var integrationEventLog in eventsToPublish)
        {
          var integrationEventType = Type.GetType(integrationEventLog.EventType);
          var integrationEvent = JsonConvert.DeserializeObject(integrationEventLog.EventBody, integrationEventType) as IntegrationEvent;
          eventBus.Publish(integrationEvent);
          await integrationEventLogService.MarkEventAsPublishedAsync(integrationEvent);
        }

        if (eventsToPublish.Count() > 0)
          await dbContext.SaveChangesAsync();

        transaction.Commit();
      }
    }

    //public override async Task StopAsync(CancellationToken stoppingToken)
    //{
    //  // Run your clean-up actions
    //}
  }
}
