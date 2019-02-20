using Biz.BrightOnion.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Events
{
  public class RoomNameChangedEvent : IntegrationEvent
  {
    public long Id { get; set; }

    public string Name { get; set; }

    public RoomNameChangedEvent(long id, string name)
    {
      Id = id;
      Name = name;
    }
  }
}
