using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Infrastructure.Services
{
  public class IdentityService : IIdentityService
  {
    private IHttpContextAccessor context;

    public IdentityService(IHttpContextAccessor context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public string GetUserIdentity()
    {
      return context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
  }
}
