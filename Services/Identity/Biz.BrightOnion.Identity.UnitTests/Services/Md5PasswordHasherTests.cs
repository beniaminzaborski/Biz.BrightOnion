using Biz.BrightOnion.Identity.API.Services;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Biz.BrightOnion.Identity.UnitTests.Services
{
  public class Md5PasswordHasherTests
  {
    [Fact]
    public void Hash_NullLogin_ShouldThrowException()
    {
      // Arrange
      var md5PasswordHasher = new Md5PasswordHasher();

      // Act
      // Assert
      Exception ex = Assert.ThrowsAny<Exception>(() => md5PasswordHasher.Hash(null, "Secret123"));
    }

    [Fact]
    public void Hash_EmptyLogin_ShouldThrowException()
    {
      // Arrange
      var md5PasswordHasher = new Md5PasswordHasher();

      // Act
      // Assert
      Exception ex = Assert.ThrowsAny<Exception>(() => md5PasswordHasher.Hash("", "Secret123"));
    }

    [Fact]
    public void Hash_WhitespaceLogin_ShouldThrowException()
    {
      // Arrange
      var md5PasswordHasher = new Md5PasswordHasher();

      // Act
      // Assert
      Exception ex = Assert.ThrowsAny<Exception>(() => md5PasswordHasher.Hash("   ", "Secret123"));
    }

    [Fact]
    public void Hash_NullPassword_ShouldThrowException()
    {
      // Arrange
      var md5PasswordHasher = new Md5PasswordHasher();

      // Act
      // Assert
      Exception ex = Assert.ThrowsAny<Exception>(() => md5PasswordHasher.Hash("jan.k@qwe.pl", null));
    }

    [Fact]
    public void Hash_EmptyPassword_ShouldThrowException()
    {
      // Arrange
      var md5PasswordHasher = new Md5PasswordHasher();

      // Act
      // Assert
      Exception ex = Assert.ThrowsAny<Exception>(() => md5PasswordHasher.Hash("jan.k@qwe.pl", ""));
    }

    [Fact]
    public void Hash_WhitespacePassword_ShouldThrowException()
    {
      // Arrange
      var md5PasswordHasher = new Md5PasswordHasher();

      // Act
      // Assert
      Exception ex = Assert.ThrowsAny<Exception>(() => md5PasswordHasher.Hash("jan.k@qwe.pl", "   "));
    }

    [Fact]
    public void Hash_PassedLoginPassword_ShouldReturnCalculatedHash()
    {
      // Arrange
      string expectedHash =  CalculateHash("jan.k@qwe.pl.Secret123");
      var md5PasswordHasher = new Md5PasswordHasher();

      // Act
      var result = md5PasswordHasher.Hash("jan.k@qwe.pl", "Secret123");

      // Assert
      Assert.Equal(expectedHash, result);
    }

    private string CalculateHash(string value)
    {
      MD5 md5 = System.Security.Cryptography.MD5.Create();
      byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(value);
      byte[] hash = md5.ComputeHash(inputBytes);

      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < hash.Length; i++)
        sb.Append(hash[i].ToString("x2"));

      string result = sb.ToString();
      return result;
    }
  }
}
