using Biz.BrightOnion.Catalog.API.Controllers;
using Biz.BrightOnion.Catalog.API.Dto;
using Biz.BrightOnion.Catalog.API.Entities;
using Biz.BrightOnion.Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Biz.BrightOnion.Catalog.UnitTests.Controller
{
  public class RoomController_Update_Tests
  {
    private readonly Mock<ISession> sessionMock;
    private readonly Mock<IRoomRepository> roomRepositoryMock;

    public RoomController_Update_Tests()
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
      var actionResult = await roomController.Update(null);

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
      var actionResult = await roomController.Update(new RoomDTO { Id = 1, Name = "" });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Room name is empty", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void RoomDoesNotExist_ShouldReturnBadRequest()
    {
      // Arrange
      roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult((Room)null));
      var roomController = new RoomController(sessionMock.Object, roomRepositoryMock.Object);

      // Act
      var actionResult = await roomController.Update(new RoomDTO { Id = 1, Name = "test" });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Room does not exist", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void OtherRoomWithPassedNameExists_ShouldReturnBadRequest()
    {
      // Arrange
      roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult(new Room() { Id = 1, Name = "test" }));
      IQueryable<Room> rooms = (new List<Room> { new Room() { Id = 2, Name = "test" } }).AsQueryable();
      roomRepositoryMock.Setup(x => x.GetAll()).Returns(rooms);
      var roomController = new RoomController(sessionMock.Object, roomRepositoryMock.Object);

      // Act
      var actionResult = await roomController.Update(new RoomDTO { Id = 1, Name = "test" });

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Room with passed name does exist", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void ShouldReturnNoContentResult()
    {
      // Arrange
      roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult(new Room() { Id = 1, Name = "test" }));
      //IQueryable<Room> rooms = (new List<Room> { new Room() { Id = 2, Name = "test" } }).AsQueryable();
      //roomRepositoryMock.Setup(x => x.GetAll()).Returns(rooms);
      var roomController = new RoomController(sessionMock.Object, roomRepositoryMock.Object);

      // Act
      var actionResult = await roomController.Update(new RoomDTO { Id = 1, Name = "test" });

      // Assert
      Assert.IsType<NoContentResult>(actionResult);
    }
  }
}
