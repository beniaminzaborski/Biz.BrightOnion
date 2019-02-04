using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biz.BrightOnion.Identity.API.Dto;
using Biz.BrightOnion.Identity.API.Infrastructure.Repositories;
using Biz.BrightOnion.Identity.API.Repositories;
using Biz.BrightOnion.Identity.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Biz.BrightOnion.Identity.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasher passwordHasher;

    public AccountController(
      IUserRepository userRepository,
      IPasswordHasher passwordHasher)
    {
      this.userRepository = userRepository;
      this.passwordHasher = passwordHasher;
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(AuthTokenDTO), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Login([FromBody]LoginDTO loginDTO)
    {
      if (loginDTO == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Login data is null" });

      if (string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email or password is empty" });

      var user = userRepository.GetAll()
        .Where(x => x.Email == loginDTO.Email).FirstOrDefault();
      if(user == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email or password is incorrect" });
      var passwordHash = passwordHasher.Hash(loginDTO.Email, loginDTO.Password);
      if(passwordHash != user.PasswordHash)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email or password is incorrect" });

      // TODO: Generate Auth Token

      return new ObjectResult(new AuthTokenDTO { Token = "1234567890" });
    }
  }
}
