using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biz.BrightOnion.Identity.API.Dto;
using Biz.BrightOnion.Identity.API.Entities;
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
    private readonly IAuthenticationService authenticationService;

    public AccountController(
      IUserRepository userRepository,
      IPasswordHasher passwordHasher,
      IAuthenticationService authenticationService)
    {
      this.userRepository = userRepository;
      this.passwordHasher = passwordHasher;
      this.authenticationService = authenticationService;
    }

    [HttpPost("register")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Register([FromBody]RegisterUserDTO registerUserDTO)
    {
      if (registerUserDTO == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Register data is null" });

      if (string.IsNullOrWhiteSpace(registerUserDTO.Email)
        || string.IsNullOrWhiteSpace(registerUserDTO.Password)
        || string.IsNullOrWhiteSpace(registerUserDTO.Password2))
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email or password is empty" });

      if (registerUserDTO.Password.Length < 7)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Password is too short" });

      if (registerUserDTO.Password != registerUserDTO.Password2)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Passwords are not the same" });

      var user = userRepository.GetByEmail(registerUserDTO.Email);
      if (user != null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "User does exist" });

      var passwordHash = passwordHasher.Hash(registerUserDTO.Email, registerUserDTO.Password);
      user = new User { Email = registerUserDTO.Email, PasswordHash = passwordHash };

      await userRepository.Create(user);

      return Ok();
    }

    [HttpPost("login")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(AuthTokenDTO), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Login([FromBody]LoginDTO loginDTO)
    {
      if (loginDTO == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Login data is null" });

      if (string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email or password is empty" });

      var user = userRepository.GetByEmail(loginDTO.Email);
      if(user == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email or password is incorrect" });
      var passwordHash = passwordHasher.Hash(loginDTO.Email, loginDTO.Password);
      if(passwordHash != user.PasswordHash)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email or password is incorrect" });

      string jwtToken = authenticationService.CreateToken(user);

      return new ObjectResult(new AuthTokenDTO { Token = jwtToken });
    }
  }
}
