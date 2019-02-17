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

namespace Biz.BrightOnion.Identity.UnitTests.Controllers
{
  public class AccountController_Login_Tests
  {
    private readonly Mock<ApplicationContext> dbContextMock;
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IPasswordHasher> passwordHasherMock;
    private readonly Mock<IAuthenticationService> authenticationService;

    public AccountController_Login_Tests()
    {
      dbContextMock = new Mock<ApplicationContext>();
      userRepositoryMock = new Mock<IUserRepository>();
      passwordHasherMock = new Mock<IPasswordHasher>();
      authenticationService = new Mock<IAuthenticationService>();
    }

    [Fact]
    public async void NullData_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = new AccountController(dbContextMock.Object, userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Login(null);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Login data is null", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void EmptyEmail_ShouldReturnBadRequest()
    {
      // Arrange
      var loginDTO = new LoginDTO { Email = "", Password = "Secret123" };
      var accountController = new AccountController(dbContextMock.Object, userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void EmptyPassword_ShouldReturnBadRequest()
    {
      // Arrange
      var loginDTO = new LoginDTO { Email = "johny.smith@exmaple-email-123.com", Password = "" };
      var accountController = new AccountController(dbContextMock.Object, userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void UserWithEmailDoesNotExist_ShouldReturnBadRequest()
    {
      // Arrange
      var loginDTO = new LoginDTO { Email = "johny.smith@exmaple-email-123.com", Password = "Secret123" };

      userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));

      var accountController = new AccountController(dbContextMock.Object, userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is incorrect", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void PassedIncorrectPassword_ShouldReturnBadRequest()
    {
      // Arrange
      var loginDTO = new LoginDTO { Email = "johny.smith@exmaple-email-123.com", Password = "Secret123" };

      userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(new User() { Id = 1, Email = "johny.smith@exmaple-email-123.com", PasswordHash = "Hash123" }));

      passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>(), It.IsAny<string>())).Returns("HashXYZ");

      var accountController = new AccountController(dbContextMock.Object, userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is incorrect", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void PassedEmailPasswd_ShouldReturnToken()
    {
      // Arrange
      var loginDTO = new LoginDTO { Email = "johny.smith@exmaple-email-123.com", Password = "Secret123" };

      userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(new User() { Id = 1, Email = "johny.smith@exmaple-email-123.com", PasswordHash = "Hash123" }));

      passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>(), It.IsAny<string>())).Returns("Hash123");

      authenticationService.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("JWTTOKEN");

      var accountController = new AccountController(dbContextMock.Object, userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var objectResult = Assert.IsType<ObjectResult>(actionResult);
      Assert.IsAssignableFrom<AuthTokenDTO>(objectResult.Value);
    }
  }
}
