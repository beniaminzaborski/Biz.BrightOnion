using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Dto
{
  public class UserDTO
  {
    public string Email { get; set; }

    public bool NotificationEnabled { get; set; }
  }
}
