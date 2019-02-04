using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Dto
{
  public class UserDTO
  {
    public long Id { get; set; }

    public bool NotificationEnabled { get; set; }
  }
}
