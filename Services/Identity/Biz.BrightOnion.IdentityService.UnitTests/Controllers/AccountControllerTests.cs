using Biz.BrightOnion.IdentityService.Controllers;
using Biz.BrightOnion.IdentityService.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Biz.BrightOnion.IdentityService.UnitTests.Controllers
{
  public class AccountControllerTests
  {
    [Fact]
    public async void Login_NullData_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = new AccountController();

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
      var accountController = new AccountController();

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password are empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Login_EmptyPassword_ShouldReturnBadRequest()
    {
      // Arrange
      var loginDTO = new LoginDTO { Email = "johny.smith@exmaple-email-123.com", Password = "" };
      var accountController = new AccountController();

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password are empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Login_PassedEmailPasswd_ShouldReturnToken()
    {
      // Arrange
      var loginDTO = new LoginDTO { Email = "johny.smith@exmaple-email-123.com", Password = "Secret123" };
      var accountController = new AccountController();

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var objectResult = Assert.IsType<ObjectResult>(actionResult);
      Assert.IsAssignableFrom<AuthTokenDTO>(objectResult.Value);
    }
  }
}
