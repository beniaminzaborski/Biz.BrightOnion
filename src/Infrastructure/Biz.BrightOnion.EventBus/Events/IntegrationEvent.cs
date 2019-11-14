using Newtonsoft.Json;
using System;

namespace Biz.BrightOnion.EventBus.Events
{
  public class IntegrationEvent
  {
    public IntegrationEvent()
    {
      EventId = Guid.NewGuid();
      EventCreationDate = DateTime.UtcNow;
    }

    [JsonConstructor]
    public IntegrationEvent(Guid eventId, DateTime eventCreateDate)
    {
      EventId = eventId;
      EventCreationDate = eventCreateDate;
    }

    [JsonProperty]
    public Guid EventId { get; private set; }

    [JsonProperty]
    public DateTime EventCreationDate { get; private set; }
  }
}
