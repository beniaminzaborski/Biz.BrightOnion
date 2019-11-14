using Biz.BrightOnion.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Events
{
  public class RoomManagerChangedEvent : IntegrationEvent
  {
    public long Id { get; set; }

    public string Name { get; set; }

    public long? ManagerId { get; set; }

    public string ManagerName { get; set; }

    public RoomManagerChangedEvent(long id, string name, long? managerId, string managerName)
    {
      Id = id;
      Name = name;
      ManagerId = managerId;
      ManagerName = managerName;
    }
  }
}
