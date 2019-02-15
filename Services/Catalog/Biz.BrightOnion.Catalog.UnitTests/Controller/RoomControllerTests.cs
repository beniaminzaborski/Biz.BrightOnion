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
  public class RoomControllerTests
  {
    private readonly Mock<ISession> sessionMock;
    private readonly Mock<IRoomRepository> roomRepositoryMock;

    public RoomControllerTests()
    {
      sessionMock = new Mock<ISession>();
      roomRepositoryMock = new Mock<IRoomRepository>();
    }

    [Fact]
    public async void GetAllAsync_ShouldReturnActionResultWithRoomDtoList()
    {
      // Arrange
      var roomController = new RoomController(sessionMock.Object, roomRepositoryMock.Object);

      // Act
      var actionResult = await roomController.GetAllAsync();

      // Assert
      Assert.IsType<ActionResult<List<RoomDTO>>>(actionResult);
    }

    [Fact]
    public async void Create_NullData_ShouldReturnBadRequest()
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
    public async void Create_EmptyName_ShouldReturnBadRequest()
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
    public async void Create_RoomWithPassedNameExists_ShouldReturnBadRequest()
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
    public async void Create_ShouldReturnOk()
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
