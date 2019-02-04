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

namespace Biz.BrightOnion.Identity.UnitTests.Controllers
{
  public class AccountControllerTests
  {
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IPasswordHasher> passwordHasherMock;
    private readonly Mock<IAuthenticationService> authenticationService;

    public AccountControllerTests()
    {
      userRepositoryMock = new Mock<IUserRepository>();
      passwordHasherMock = new Mock<IPasswordHasher>();
      authenticationService = new Mock<IAuthenticationService>();
    }

    [Fact]
    public async void Register_NullData_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Register(null);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Register data is null", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Register_EmptyEmail_ShouldReturnBadRequest()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "", Password = "Secret123", Password2 = "Secret123" };
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Register(registerUserDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Register_EmptyPassword_ShouldReturnBadRequest()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "jan.test@asf.pl", Password = "", Password2 = "Secret123" };
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Register(registerUserDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Register_EmptyPassword2_ShouldReturnBadRequest()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "jan.test@asf.pl", Password = "Secret123", Password2 = "" };
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Register(registerUserDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Email or password is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Register_PasswordShorterThan7_ShouldReturnBadRequest()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "jan.test@asf.pl", Password = "123", Password2 = "123" };
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Register(registerUserDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Password is too short", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Register_PasswordsAreNotTheSame_ShouldReturnBadRequest()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "jan.test@asf.pl", Password = "secret123", Password2 = "secretXXX" };
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Register(registerUserDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Passwords are not the same", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Register_UserExists_ShouldReturnBadRequest()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "jan.test@asf.pl", Password = "secret123", Password2 = "secret123" };

      userRepositoryMock.Setup(x => x.GetByEmail(It.IsAny<string>())).Returns(new User() { Id = 1, Email = "jan.test@asf.pl", PasswordHash = "Hash123" });

      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Register(registerUserDTO);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("User does exist", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Register_ShouldReturnOk()
    {
      // Arrange
      var registerUserDTO = new RegisterUserDTO { Email = "jan.test@asf.pl", Password = "secret123", Password2 = "secret123" };

      userRepositoryMock.Setup(x => x.GetByEmail(It.IsAny<string>())).Returns((User)null);

      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Register(registerUserDTO);

      // Assert
      var objectResult = Assert.IsType<OkResult>(actionResult);
    }

    [Fact]
    public async void Login_NullData_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

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
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

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
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

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

      userRepositoryMock.Setup(x => x.GetByEmail(It.IsAny<string>())).Returns((User)null);

      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

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

      userRepositoryMock.Setup(x => x.GetByEmail(It.IsAny<string>())).Returns(new User() { Id = 1, Email = "johny.smith@exmaple-email-123.com", PasswordHash = "Hash123" });

      passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>(), It.IsAny<string>())).Returns("HashXYZ");

      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

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

      userRepositoryMock.Setup(x => x.GetByEmail(It.IsAny<string>())).Returns(new User() { Id = 1, Email = "johny.smith@exmaple-email-123.com", PasswordHash = "Hash123" });

      passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>(), It.IsAny<string>())).Returns("Hash123");

      authenticationService.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("JWTTOKEN");

      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Login(loginDTO);

      // Assert
      var objectResult = Assert.IsType<ObjectResult>(actionResult);
      Assert.IsAssignableFrom<AuthTokenDTO>(objectResult.Value);
    }

    [Fact]
    public async void Update_NullData_ShouldReturnBadRequest()
    {
      // Arrange
      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Update(null);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("User data is null", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Update_UserDoesNotExist_ShouldReturnBadRequest()
    {
      // Arrange
      userRepositoryMock.Setup(x => x.GetById(It.IsAny<long>())).Returns(Task.FromResult<User>(null));

      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Update(new UserDTO { Id = 1, NotificationEnabled = true });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("User does not exist", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void Update_ShouldReturnOk()
    {
      // Arrange
      userRepositoryMock.Setup(x => x.GetById(It.IsAny<long>())).Returns(Task.FromResult<User>(new User { Id = 1, Email = "jan.test@123.pl", NotificationEnabled = false }));

      var accountController = new AccountController(userRepositoryMock.Object, passwordHasherMock.Object, authenticationService.Object);

      // Act
      var actionResult = await accountController.Update(new UserDTO { Id = 1, NotificationEnabled = true });

      // Assert
      Assert.IsType<OkResult>(actionResult);
    }
  }
}
