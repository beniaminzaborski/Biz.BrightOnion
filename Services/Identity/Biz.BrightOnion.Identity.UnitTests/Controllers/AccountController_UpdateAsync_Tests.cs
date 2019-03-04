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
  public class AccountController_UpdateAsync_Tests
  {
    private readonly Mock<DatabaseFacade> databaseFacadeMock;
    private readonly Mock<ApplicationContext> dbContextMock;
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IPasswordHasher> passwordHasherMock;
    private readonly Mock<IAuthenticationService> authenticationService;
    private readonly Mock<IIntegrationEventLogService> integrationEventLogServiceMock;
    private readonly Mock<IEventBus> eventBusMock;

    public AccountController_UpdateAsync_Tests()
    {
      dbContextMock = new Mock<ApplicationContext>();
      databaseFacadeMock = new Mock<DatabaseFacade>(dbContextMock.Object);
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
      var actionResult = await accountController.UpdateAsync(null);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("User data is null", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void UserDoesNotExist_ShouldReturnBadRequest()
    {
      // Arrange
      userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult<User>(null));

      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.UpdateAsync(new UserDTO { Id = 1, NotificationEnabled = true });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("User does not exist", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void ShouldReturnOk()
    {
      // Arrange
      dbContextMock.SetupGet(x => x.Database).Returns(databaseFacadeMock.Object);
      databaseFacadeMock.Setup(x => x.BeginTransaction()).Returns((new Mock<IDbContextTransaction>()).Object);
      userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult<User>(new User { Id = 1, Email = "jan.test@123.pl", NotificationEnabled = false }));

      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.UpdateAsync(new UserDTO { Id = 1, NotificationEnabled = true });

      // Assert
      Assert.IsType<OkResult>(actionResult);
    }

    [Fact]
    public async void ShouldSaveUserNotificationChangedEventToPublish()
    {
      // Arrange
      dbContextMock.SetupGet(x => x.Database).Returns(databaseFacadeMock.Object);
      databaseFacadeMock.Setup(x => x.BeginTransaction()).Returns((new Mock<IDbContextTransaction>()).Object);
      userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult<User>(new User { Id = 1, Email = "jan.test@123.pl", NotificationEnabled = false }));

      var accountController = CreateAccountController();

      // Act
      var actionResult = await accountController.UpdateAsync(new UserDTO { Id = 1, NotificationEnabled = true });

      // Assert
      integrationEventLogServiceMock.Verify(x => x.SaveEventAsync(It.IsAny<IntegrationEvent>()));
    }
  }
}
