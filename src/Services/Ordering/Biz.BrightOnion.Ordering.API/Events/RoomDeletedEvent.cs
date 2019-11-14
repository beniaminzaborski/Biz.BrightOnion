using Biz.BrightOnion.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Events
{
  public class RoomDeletedEvent : IntegrationEvent
  {
    public long Id { get; set; }

    public RoomDeletedEvent(long id)
    {
      Id = id;
    }
  }
}
