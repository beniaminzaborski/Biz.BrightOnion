using Biz.BrightOnion.Catalog.API.Controllers;
using Biz.BrightOnion.Catalog.API.Dto;
using Biz.BrightOnion.Catalog.API.Entities;
using Biz.BrightOnion.Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Biz.BrightOnion.Catalog.UnitTests.Controller
{
  public class RoomController_Create_Tests
  {
    private readonly Mock<ISession> sessionMock;
    private readonly Mock<IRoomRepository> roomRepositoryMock;

    public RoomController_Create_Tests()
    {
      sessionMock = new Mock<ISession>();
      roomRepositoryMock = new Mock<IRoomRepository>();
    }

    [Fact]
    public async void NullData_ShouldReturnBadRequest()
    {
      // Arrange
      var roomController = new RoomController(sessionMock.Object, roomRepositoryMock.Object);

      // Act
      var actionResult = await roomController.Create(null);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Room data is null", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void EmptyName_ShouldReturnBadRequest()
    {
      // Arrange
      var roomController = new RoomController(sessionMock.Object, roomRepositoryMock.Object);

      // Act
      var actionResult = await roomController.Create(new RoomDTO { Name = "" });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Room name is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void RoomWithPassedNameExists_ShouldReturnBadRequest()
    {
      // Arrange
      roomRepositoryMock.Setup(x => x.GetByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new Room() { Id = 1, Name = "test" }));
      var roomController = new RoomController(sessionMock.Object, roomRepositoryMock.Object);

      // Act
      var actionResult = await roomController.Create(new RoomDTO { Name = "test" });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Room does exist", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void ShouldReturnOk()
    {
      // Arrange
      var roomDTO = new RoomDTO { Name = "test" };
      var roomController = new RoomController(sessionMock.Object, roomRepositoryMock.Object);

      // Act
      var actionResult = await roomController.Create(roomDTO);

      // Assert
      var objectResult = Assert.IsType<ObjectResult>(actionResult);
      Assert.IsAssignableFrom<RoomDTO>(objectResult.Value);
    }
  }
}
