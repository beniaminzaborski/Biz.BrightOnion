using Biz.BrightOnion.Identity.API.Entities;
using Biz.BrightOnion.Identity.API.Infrastructure.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Specifications
{
  public class UserWithNotificationEnabledSpecification : BaseSpecification<User>
  {
    public UserWithNotificationEnabledSpecification(int basketId)
    : base(u => u.NotificationEnabled == true) { }
  }
}
