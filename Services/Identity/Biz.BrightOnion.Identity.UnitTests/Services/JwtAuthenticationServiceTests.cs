using Biz.BrightOnion.Identity.API.Configuration;
using Biz.BrightOnion.Identity.API.Entities;
using Biz.BrightOnion.Identity.API.Services;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Biz.BrightOnion.Identity.UnitTests.Services
{
  public class JwtAuthenticationServiceTests
  {
    // private Mock<IOptions<AppSettings>> appSettingsMock;
    private Mock<AppSettings> appSettingsMock;
    private Mock<IOptions<AppSettings>> optionsMock;

    public JwtAuthenticationServiceTests()
    {
      appSettingsMock = new Mock<AppSettings>();
      optionsMock = new Mock<IOptions<AppSettings>>();
    }

    [Fact]
    public void CreateToken_NullUser_ShouldThrowArgumentException()
    {
      // Arrange
      appSettingsMock.SetupGet(x => x.Secret).Returns("012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
      optionsMock.SetupGet(x => x.Value).Returns(appSettingsMock.Object);
      var jwtAuthenticationService = new JwtAuthenticationService(optionsMock.Object);

      // Act
      // Assert
      Assert.Throws<ArgumentException>(() => jwtAuthenticationService.CreateToken(null));
    }

    [Fact]
    public void CreateToken_PassedNotNullUser_ShouldReturnNotEmptyToken()
    {
      // Arrange
      appSettingsMock.SetupGet(x => x.Secret).Returns("012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
      optionsMock.SetupGet(x => x.Value).Returns(appSettingsMock.Object);
      var jwtAuthenticationService = new JwtAuthenticationService(optionsMock.Object);

      // Act
      var result = jwtAuthenticationService.CreateToken(new User { Id = 1 });

      // Assert
      Assert.NotNull(result);
      Assert.False(string.IsNullOrWhiteSpace(result));
    }
  }
}
