using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Dto
{
  public class RegisterUserDTO
  {
    public string Email { get; set; }

    public string Password { get; set; }

    public string Password2 { get; set; }
  }
}
