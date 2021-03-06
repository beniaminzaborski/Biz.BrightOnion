﻿using Biz.BrightOnion.Identity.API.Infrastructure.Repositories;
using Biz.BrightOnion.Identity.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Repositories
{
  public interface IUserRepository : IRepository<User>
  {
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAll();
  }
}
