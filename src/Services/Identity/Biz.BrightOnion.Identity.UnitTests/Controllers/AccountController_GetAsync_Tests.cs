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
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Biz.BrightOnion.EventBus.Events;

namespace Biz.BrightOnion.Identity.UnitTests.Controllers
{
  public class AccountController_GetAsync_Tests
  {
    private readonly Mock<DatabaseFacade> databaseFacadeMock;
    private readonly Mock<ApplicationContext> dbContextMock;
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IPasswordHasher> passwordHasherMock;
    private readonly Mock<IAuthenticationService> authenticationService;
    private readonly Mock<IIntegrationEventLogService> integrationEventLogServiceMock;

    public AccountController_GetAsync_Tests()
    {
      dbContextMock = new Mock<ApplicationContext>();
      databaseFacadeMock = new Mock<DatabaseFacade>(dbContextMock.Object);
      userRepositoryMock = new Mock<IUserRepository>();
      passwordHasherMock = new Mock<IPasswordHasher>();
      authenticationService = new Mock<IAuthenticationService>();
      integrationEventLogServiceMock = new Mock<IIntegrationEventLogService>();
    }

    private AccountController CreateAccountController()
    {
      return new AccountController(dbContextMock.Object, userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object, integrationEventLogServiceMock.Object);
    }

    [Fact]
    public async void NullData_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.GetAsync(null);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email is null", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void UserDoesNotExist_ShouldReturnNotFound()
    {
      // Arrange
      userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult<User>(null));

      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.GetAsync("jan.test@123.pl");

      // Assert
      var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(notFoundObjectResult.Value);
      Assert.Equal("User does not exist", ((ErrorDTO)notFoundObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void ShouldReturnOkObject()
    {
      // Arrange
      dbContextMock.SetupGet(x => x.Database).Returns(databaseFacadeMock.Object);
      databaseFacadeMock.Setup(x => x.BeginTransaction()).Returns((new Mock<IDbContextTransaction>()).Object);
      userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(new User { Id = 1, Email = "jan.test@123.pl", NotificationEnabled = false }));

      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.GetAsync("jan.test@123.pl");

      // Assert
      var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
      Assert.IsAssignableFrom<UserDTO>(okObjectResult.Value);
    }
  }
}
