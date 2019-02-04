using Biz.BrightOnion.Identity.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Services
{
  public interface IAuthenticationService
  {
    string CreateToken(User user);
  }
}
