using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.Identity.API.Data;
using Biz.BrightOnion.Identity.API.Dto;
using Biz.BrightOnion.Identity.API.Entities;
using Biz.BrightOnion.Identity.API.Events;
using Biz.BrightOnion.Identity.API.Repositories;
using Biz.BrightOnion.Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biz.BrightOnion.Identity.API.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    private readonly ApplicationContext dbContext;
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly IAuthenticationService authenticationService;
    private readonly IIntegrationEventLogService integrationEventLogService;
    private readonly IEventBus eventBus;

    public AccountController(
      ApplicationContext dbContext,
      IUserRepository userRepository,
      IPasswordHasher passwordHasher,
      IAuthenticationService authenticationService,
      IIntegrationEventLogService integrationEventLogService,
      IEventBus eventBus)
    {
      this.dbContext = dbContext;
      this.userRepository = userRepository;
      this.passwordHasher = passwordHasher;
      this.authenticationService = authenticationService;
      this.integrationEventLogService = integrationEventLogService;
      this.eventBus = eventBus;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> RegisterAsync([FromBody]RegisterUserDTO registerUserDTO)
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
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Passwords are not equal" });

      var user = await userRepository.GetByEmailAsync(registerUserDTO.Email);
      if (user != null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "User does exist" });

      var passwordHash = passwordHasher.Hash(registerUserDTO.Email, registerUserDTO.Password);
      user = new User { Email = registerUserDTO.Email, PasswordHash = passwordHash };

      await userRepository.CreateAsync(user);
      await dbContext.SaveChangesAsync();

      return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(AuthTokenDTO), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> LoginAsync([FromBody]LoginDTO loginDTO)
    {
      if (loginDTO == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Login data is null" });

      if (string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email or password is empty" });

      var user = await userRepository.GetByEmailAsync(loginDTO.Email);
      if(user == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email or password is incorrect" });
      var passwordHash = passwordHasher.Hash(loginDTO.Email, loginDTO.Password);
      if(passwordHash != user.PasswordHash)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email or password is incorrect" });

      string jwtToken = authenticationService.CreateToken(user);

      return new ObjectResult(new AuthTokenDTO { UserId = user.Id, Token = jwtToken });
    }

    [HttpPost("changepassword")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody]ChangePasswordDTO changePasswordDTO)
    {
      if (changePasswordDTO == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Password data is null" });

      if (string.IsNullOrWhiteSpace(changePasswordDTO.Email))
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email is empty" });

      if (string.IsNullOrWhiteSpace(changePasswordDTO.Password) 
        || string.IsNullOrWhiteSpace(changePasswordDTO.Password2)
        || string.IsNullOrWhiteSpace(changePasswordDTO.OldPassword))
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Passwords are empty" });

      var user = await userRepository.GetByEmailAsync(changePasswordDTO.Email);
      if (user == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "User does not exist" });
      var passwordHash = passwordHasher.Hash(changePasswordDTO.Email, changePasswordDTO.OldPassword);
      if (passwordHash != user.PasswordHash)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email or password is incorrect" });

      if(changePasswordDTO.Password != changePasswordDTO.Password2)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Passwords are not equal" });

      passwordHash = passwordHasher.Hash(changePasswordDTO.Email, changePasswordDTO.Password);
      user.PasswordHash = passwordHash;
      userRepository.Update(user.Id, user);
      await dbContext.SaveChangesAsync();

      return NoContent();
    }

    [HttpGet("{email}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetAsync(string email)
    {
      if (string.IsNullOrWhiteSpace(email))
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Email is null" });

      var user = await userRepository.GetByEmailAsync(email);

      if (user == null)
        return new NotFoundObjectResult(new ErrorDTO { ErrorMessage = "User does not exist" });

      return Ok(new UserDTO { Email = user.Email, NotificationEnabled = user.NotificationEnabled });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserItemDTO>), (int)HttpStatusCode.OK)]
    public async Task<IEnumerable<UserItemDTO>> GetAllAsync()
    {
      var users = await userRepository.GetAll();
      return users.Select(u => new UserItemDTO { UserId = u.Id, Email = u.Email })
        .ToList();
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateAsync([FromBody]UserDTO userDTO)
    {
      if (userDTO == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "User data is null" });

      var user = await userRepository.GetByEmailAsync(userDTO.Email);
      if(user == null)
        return new NotFoundObjectResult(new ErrorDTO { ErrorMessage = "User does not exist" });

      if (user.NotificationEnabled != userDTO.NotificationEnabled)
      {
        user.NotificationEnabled = userDTO.NotificationEnabled;

        var userNotificationChangedEvent = new UserNotificationChangedEvent(user.Id, user.NotificationEnabled);

        using (var transaction = dbContext.Database.BeginTransaction())
        {
          userRepository.Update(user.Id, user);
          await integrationEventLogService.SaveEventAsync(userNotificationChangedEvent);
          await dbContext.SaveChangesAsync();
          transaction.Commit();
        }
      }

      return NoContent();
    }
  }
}
