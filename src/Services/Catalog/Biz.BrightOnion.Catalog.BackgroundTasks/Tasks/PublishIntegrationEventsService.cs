using Biz.BrightOnion.Catalog.BackgroundTasks.Configuration;
using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.EventBus.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.BackgroundTasks.Tasks
{
  public class PublishIntegrationEventsService : BackgroundService
  {
    private readonly ILogger<PublishIntegrationEventsService> logger;
    private readonly IEventBus eventBus;
    private readonly string connectionString;
    private readonly int publishEventBatchSize;

    public PublishIntegrationEventsService(
      IConfiguration configuration,
      ILogger<PublishIntegrationEventsService> logger,
      IEventBus eventBus)
    {
      this.logger = logger;
      this.eventBus = eventBus;

      this.connectionString = configuration.GetConnectionString("DefaultConnection");
      var appSettingsSection = configuration.GetSection("AppSettings");
      var appSettings = appSettingsSection.Get<AppSettings>();
      publishEventBatchSize = appSettings.PublishEventBatchSize ?? 10;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      logger.LogDebug($"PublishIntegrationEventsService is starting.");

      stoppingToken.Register(() =>
        logger.LogDebug($"PublishIntegrationEventsService background task is stopping."));

      while (!stoppingToken.IsCancellationRequested)
      {
        logger.LogDebug($"PublishIntegrationEventsService task doing background work.");
        
        await PublishUnpublishedIntegrationEvents();

        await Task.Delay(10000, stoppingToken);
      }

      logger.LogDebug($"PublishIntegrationEventsService background task is stopping.");
    }

    private async Task<IDictionary<long, IntegrationEvent>> GetUnpublishedIntegrationEvents()
    {
      IDictionary<long, IntegrationEvent> eventsToPublish = new Dictionary<long, IntegrationEvent>();

      using (var connection = new SqlConnection(connectionString))
      {
        try
        {
          await connection.OpenAsync();

          var sqlQuery = @"
            SELECT TOP (@count) Id, EventType, EventBody FROM dbo.IntegrationEventLog
            WHERE State = 'ReadyToPublish'
            ORDER BY EventCreationDate DESC";

          using (var command = new SqlCommand(sqlQuery, connection))
          {
            command.Parameters.AddWithValue("@count", publishEventBatchSize);

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
              long id = reader.GetInt64(0);
              string eventType = reader.GetString(1);
              string eventBody = reader.GetString(2);

              var integrationEventType = Type.GetType(eventType);
              var integrationEvent = JsonConvert.DeserializeObject(eventBody, integrationEventType) as IntegrationEvent;

              eventsToPublish.Add(id, integrationEvent);
            }
          }
        }
        catch (SqlException exception)
        {
          logger.LogCritical(exception, "FATAL ERROR: Database connections could not be opened: {Message}", exception.Message);
        }
      }

      return eventsToPublish;
    }

    private async Task PublishUnpublishedIntegrationEvents()
    {
      var eventsToPublish = await GetUnpublishedIntegrationEvents();

      if (eventsToPublish.Count == 0)
        return;

      using (var connection = new SqlConnection(connectionString))
      {
        try
        {
          await connection.OpenAsync();

          foreach (var eventToPublish in eventsToPublish)
          {
            eventBus.Publish(eventToPublish.Value);
            await MarkEventAsPublishedAsync(connection, eventToPublish.Key);
          }
        }
        catch (Exception exception)
        {
          logger.LogCritical(exception, "FATAL ERROR: Database connections could not be opened: {Message}", exception.Message);
        }
      }
    }

    private async Task MarkEventAsPublishedAsync(SqlConnection connection, long eventLogId)
    {
      string sqlQuery = @"UPDATE dbo.IntegrationEventLog SET State = 'Published' WHERE Id = @id";
      using (var command = new SqlCommand(sqlQuery, connection))
      {
        command.Parameters.AddWithValue("@id", eventLogId);
        await command.ExecuteNonQueryAsync();
      }
    }

    //public override async Task StopAsync(CancellationToken stoppingToken)
    //{
    //  // Run your clean-up actions
    //}
  }
}
