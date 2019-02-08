using Biz.BrightOnion.Identity.API.Data;
using Biz.BrightOnion.Identity.API.Infrastructure.Repositories;
using Biz.BrightOnion.Identity.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NGuard;

namespace Biz.BrightOnion.Identity.API.Repositories
{
  public class UserRepository : Repository<User>, IUserRepository
  {
    public UserRepository(ApplicationContext dbContext) : base(dbContext) { }

    public async Task<User> GetByEmailAsync(string email)
    {
      Guard.Requires(email, nameof(email)).IsNotNullOrEmptyOrWhiteSpace();

      return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
  }
}
