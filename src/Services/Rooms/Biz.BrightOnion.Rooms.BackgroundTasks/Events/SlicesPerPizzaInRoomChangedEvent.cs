using Biz.BrightOnion.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Rooms.API.Events
{
  public class SlicesPerPizzaInRoomChangedEvent : IntegrationEvent
  {
    public long Id { get; set; }

    public string Name { get; set; }

    public int SlicesPerPizza { get; set; }

    public SlicesPerPizzaInRoomChangedEvent(long id, string name, int slicesPerPizza)
    {
      Id = id;
      Name = name;
      SlicesPerPizza = slicesPerPizza;
    }
  }
}
