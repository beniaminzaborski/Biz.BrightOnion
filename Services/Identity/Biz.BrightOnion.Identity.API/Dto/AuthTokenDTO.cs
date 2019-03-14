using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Dto
{
  public class AuthTokenDTO
  {
    public long UserId { get; set; }
    public string Token { get; set; }
  }
}
