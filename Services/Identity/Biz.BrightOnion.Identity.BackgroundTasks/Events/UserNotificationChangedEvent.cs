using Biz.BrightOnion.EventBus.Events;

namespace Biz.BrightOnion.Identity.API.Events
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
