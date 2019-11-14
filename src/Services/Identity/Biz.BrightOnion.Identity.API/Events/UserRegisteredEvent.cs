using Biz.BrightOnion.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
