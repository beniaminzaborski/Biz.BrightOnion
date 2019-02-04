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

namespace Biz.BrightOnion.Identity.UnitTests.Controllers
{
  public class AccountControllerTests
  {
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IPasswordHasher> passwordHasherMock;

    public AccountControllerTests()
    {
      userRepositoryMock = new Mock<IUserRepository>();
      passwordHasherMock = new Mock<IPasswordHasher>();
    }

    [Fact]
    public async void Login_NullData_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object);

      // Act
      var actionResult = await accountController.Login(null);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Login data is null", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Login_EmptyEmail_ShouldReturnBadRequest()
    {
      // Arrange
      var loginDTO = new LoginDTO { Email = "", Password = "Secret123" };
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object);

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Login_EmptyPassword_ShouldReturnBadRequest()
    {
      // Arrange
      var loginDTO = new LoginDTO { Email = "johny.smith@exmaple-email-123.com", Password = "" };
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object);

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Login_UserWithEmailDoesNotExist_ShouldReturnBadRequest()
    {
      // Arrange
      var loginDTO = new LoginDTO { Email = "johny.smith@exmaple-email-123.com", Password = "Secret123" };

      var users = new List<User> { };
      userRepositoryMock.Setup(x => x.GetAll()).Returns(users.AsQueryable());

      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object);

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is incorrect", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Login_PassedIncorrectPassword_ShouldReturnBadRequest()
    {
      // Arrange
      var loginDTO = new LoginDTO { Email = "johny.smith@exmaple-email-123.com", Password = "Secret123" };

      var users = new List<User> { new User() { Id = 1, Email = "johny.smith@exmaple-email-123.com", PasswordHash = "Hash123" } };
      userRepositoryMock.Setup(x => x.GetAll()).Returns(users.AsQueryable());

      passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>(), It.IsAny<string>())).Returns("HashXYZ");

      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object);

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is incorrect", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Login_PassedEmailPasswd_ShouldReturnToken()
    {
      // Arrange
      var loginDTO = new LoginDTO { Email = "johny.smith@exmaple-email-123.com", Password = "Secret123" };

      var users = new List<User> { new User() { Id = 1, Email = "johny.smith@exmaple-email-123.com", PasswordHash = "Hash123" } };
      userRepositoryMock.Setup(x => x.GetAll()).Returns(users.AsQueryable());

      passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>(), It.IsAny<string>())).Returns("Hash123");

      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object);

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var objectResult = Assert.IsType<ObjectResult>(actionResult);
      Assert.IsAssignableFrom<AuthTokenDTO>(objectResult.Value);
    }
  }
}
