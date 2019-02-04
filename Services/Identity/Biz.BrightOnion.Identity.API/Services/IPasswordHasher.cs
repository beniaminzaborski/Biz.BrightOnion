using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Services
{
  public interface IPasswordHasher
  {
    string Hash(string login, string password);
  }
}
