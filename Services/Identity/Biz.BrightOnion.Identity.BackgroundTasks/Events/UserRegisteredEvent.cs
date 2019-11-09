using Biz.BrightOnion.EventBus.Events;

namespace Biz.BrightOnion.Identity.API.Events
{
    public class UserRegisteredEvent : IntegrationEvent
  {
    public long Id { get; set; }

    public string Email { get; set; }

    public UserRegisteredEvent(long id, string email)
    {
      Id = id;
      Email = email;
    }
  }
}
