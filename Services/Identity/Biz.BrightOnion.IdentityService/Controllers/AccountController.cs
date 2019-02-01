using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biz.BrightOnion.IdentityService.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Biz.BrightOnion.IdentityService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    [HttpPost]
    public async Task<IActionResult> Login([FromBody]LoginDTO loginDTO)
    {
      if (loginDTO == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Login data is null" });

      if (string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email or password are empty" });

      // TODO: Implement authentication

      return new ObjectResult(new AuthTokenDTO { Token = "1234567890" });
    }
  }
}
