﻿using Biz.BrightOnion.Identity.API.Controllers;
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
  public class AccountController_ChangePasswordAsync_Tests
  {
    private readonly Mock<ApplicationContext> dbContextMock;
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IPasswordHasher> passwordHasherMock;
    private readonly Mock<IAuthenticationService> authenticationService;
    private readonly Mock<IIntegrationEventLogService> integrationEventLogServiceMock;
    private readonly Mock<IEventBus> eventBusMock;

    public AccountController_ChangePasswordAsync_Tests()
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
      var actionResult = await accountController.ChangePasswordAsync(null);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Password data is null", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void EmptyEmail_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.ChangePasswordAsync(new ChangePasswordDTO { Email = "" });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void EmptyPassword_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.ChangePasswordAsync(new ChangePasswordDTO { Email = "jan.test@tyu.pl" });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Passwords are empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void EmptyPassword2_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.ChangePasswordAsync(new ChangePasswordDTO { Email = "jan.test@tyu.pl", Password = "123321" });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Passwords are empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void EmptyOldPassword_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.ChangePasswordAsync(new ChangePasswordDTO { Email = "jan.test@tyu.pl", Password = "123321", Password2 = "123321" });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Passwords are empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void UserDoesNotExist_ShouldReturnBadRequest()
    {
      // Arrange
      userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult<User>(null));

      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.ChangePasswordAsync(new ChangePasswordDTO { Email = "jan.test@tyu.pl", Password = "123321", Password2 = "123321", OldPassword = "asddsa" });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("User does not exist", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void IncorrectOldPassword_ShouldReturnBadRequest()
    {
      // Arrange
      userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(new User { Id = 1, Email = "jan.test@tyu.pl", PasswordHash = "Hash123" }));

      passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>(), It.IsAny<string>())).Returns("HashXYZ");

      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.ChangePasswordAsync(new ChangePasswordDTO { Email = "jan.test@tyu.pl", Password = "123321", Password2 = "123321", OldPassword = "asddsa" });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is incorrect", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void PasswordsAreNotEqual_ShouldReturnBadRequest()
    {
      // Arrange
      userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(new User { Id = 1, Email = "jan.test@tyu.pl", PasswordHash = "HashXYZ" }));

      passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>(), It.IsAny<string>())).Returns("HashXYZ");

      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.ChangePasswordAsync(new ChangePasswordDTO { Email = "jan.test@tyu.pl", Password = "123321", Password2 = "123___", OldPassword = "asddsa" });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Passwords are not equal", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void ShouldReturnNoContent()
    {
      // Arrange
      userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(new User { Id = 1, Email = "jan.test@tyu.pl", PasswordHash = "HashXYZ" }));

      passwordHasherMock.Setup(x => x.Hash("jan.test@tyu.pl", "asddsa")).Returns("HashXYZ");
      passwordHasherMock.Setup(x => x.Hash("jan.test@tyu.pl", "123321")).Returns("Hash123");

      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.ChangePasswordAsync(new ChangePasswordDTO { Email = "jan.test@tyu.pl", Password = "123321", Password2 = "123321", OldPassword = "asddsa" });

      // Assert
      Assert.IsType<NoContentResult>(actionResult);
    }
  }
}
