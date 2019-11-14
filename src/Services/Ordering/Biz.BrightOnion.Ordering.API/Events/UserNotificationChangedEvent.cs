using Biz.BrightOnion.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Events
{
  public class UserNotificationChangedEvent : IntegrationEvent
  {
    public long Id { get; set; }

    public bool NotificationEnabled { get; set; }

    public UserNotificationChangedEvent(long id, bool notificationEnabled)
    {
      Id = id;
      NotificationEnabled = notificationEnabled;
    }
  }
}
