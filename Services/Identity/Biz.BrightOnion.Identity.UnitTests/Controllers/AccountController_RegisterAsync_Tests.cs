using Biz.BrightOnion.Identity.API.Controllers;
using Biz.BrightOnion.Identity.API.Dto;
using Biz.BrightOnion.Identity.API.Infrastructure.Repositories;
using Biz.BrightOnion.Identity.API.Repositories;
using Biz.BrightOnion.Identity.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using Biz.BrightOnion.Identity.API.Entities;
using System.Threading.Tasks;
using Biz.BrightOnion.Identity.API.Data;
using Biz.BrightOnion.EventBus.Abstractions;

namespace Biz.BrightOnion.Identity.UnitTests.Controllers
{
  public class AccountController_RegisterAsync_Tests
  {
    private readonly Mock<ApplicationContext> dbContextMock;
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IPasswordHasher> passwordHasherMock;
    private readonly Mock<IAuthenticationService> authenticationService;
    private readonly Mock<IIntegrationEventLogService> integrationEventLogServiceMock;
    private readonly Mock<IEventBus> eventBusMock;

    public AccountController_RegisterAsync_Tests()
    {
      dbContextMock = new Mock<ApplicationContext>();
      userRepositoryMock = new Mock<IUserRepository>();
      passwordHasherMock = new Mock<IPasswordHasher>();
      authenticationService = new Mock<IAuthenticationService>();
      integrationEventLogServiceMock = new Mock<IIntegrationEventLogService>();
      eventBusMock = new Mock<IEventBus>();
    }

    private AccountController CreateAccountController()
    {
      return new AccountController(dbContextMock.Object, userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object, integrationEventLogServiceMock.Object, eventBusMock.Object);
    }

    [Fact]
    public async void NullData_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.RegisterAsync(null);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Register data is null", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void EmptyEmail_ShouldReturnBadRequest()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "", Password = "Secret123", Password2 = "Secret123" };
      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.RegisterAsync(registerUserDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void EmptyPassword_ShouldReturnBadRequest()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "jan.test@asf.pl", Password = "", Password2 = "Secret123" };
      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.RegisterAsync(registerUserDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void EmptyPassword2_ShouldReturnBadRequest()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "jan.test@asf.pl", Password = "Secret123", Password2 = "" };
      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.RegisterAsync(registerUserDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void PasswordShorterThan7_ShouldReturnBadRequest()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "jan.test@asf.pl", Password = "123", Password2 = "123" };
      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.RegisterAsync(registerUserDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Password is too short", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void PasswordsAreNotTheSame_ShouldReturnBadRequest()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "jan.test@asf.pl", Password = "secret123", Password2 = "secretXXX" };
      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.RegisterAsync(registerUserDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Passwords are not equal", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void UserExists_ShouldReturnBadRequest()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "jan.test@asf.pl", Password = "secret123", Password2 = "secret123" };

      userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(new User() { Id = 1, Email = "jan.test@asf.pl", PasswordHash = "Hash123" }));

      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.RegisterAsync(registerUserDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("User does exist", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void ShouldReturnOk()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "jan.test@asf.pl", Password = "secret123", Password2 = "secret123" };

      userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));

      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.RegisterAsync(registerUserDTO);

      // Assert
      var objectResult = Assert.IsType<OkResult>(actionResult);
    }
  }
}
