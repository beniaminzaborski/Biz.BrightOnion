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
  public class AccountController_UpdateAsync_Tests
  {
    private readonly Mock<ApplicationContext> dbContextMock;
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IPasswordHasher> passwordHasherMock;
    private readonly Mock<IAuthenticationService> authenticationService;
    private readonly Mock<IEventBus> eventBusMock;

    public AccountController_UpdateAsync_Tests()
    {
      dbContextMock = new Mock<ApplicationContext>();
      userRepositoryMock = new Mock<IUserRepository>();
      passwordHasherMock = new Mock<IPasswordHasher>();
      authenticationService = new Mock<IAuthenticationService>();
      eventBusMock = new Mock<IEventBus>();
    }

    [Fact]
    public async void NullData_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = new AccountController(dbContextMock.Object, userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object, eventBusMock.Object);

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

      var accountController = new AccountController(dbContextMock.Object, userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object, eventBusMock.Object);

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
      userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult<User>(new User { Id = 1, Email = "jan.test@123.pl", NotificationEnabled = false }));

      var accountController = new AccountController(dbContextMock.Object, userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object, eventBusMock.Object);

      // Act
      var actionResult = await accountController.UpdateAsync(new UserDTO { Id = 1, NotificationEnabled = true });

      // Assert
      Assert.IsType<OkResult>(actionResult);
    }
  }
}
