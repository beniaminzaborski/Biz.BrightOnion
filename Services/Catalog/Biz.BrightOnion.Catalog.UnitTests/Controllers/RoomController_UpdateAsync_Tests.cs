using Biz.BrightOnion.Catalog.API.Controllers;
using Biz.BrightOnion.Catalog.API.Dto;
using Biz.BrightOnion.Catalog.API.Entities;
using Biz.BrightOnion.Catalog.API.Repositories;
using Biz.BrightOnion.Catalog.API.Services;
using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.EventBus.Events;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Biz.BrightOnion.Catalog.UnitTests.Controller
{
    public class RoomController_UpdateAsync_Tests
  {
    private readonly Mock<ISession> sessionMock;
    private readonly Mock<IRoomRepository> roomRepositoryMock;
    private readonly Mock<IIntegrationEventLogService> integrationEventLogServiceMock;
    private readonly Mock<IEventBus> eventBusMock;

    public RoomController_UpdateAsync_Tests()
    {
      sessionMock = new Mock<ISession>();
      roomRepositoryMock = new Mock<IRoomRepository>();
      integrationEventLogServiceMock = new Mock<IIntegrationEventLogService>();
      eventBusMock = new Mock<IEventBus>();
    }

    private RoomController CreateRoomController()
    {
      return new RoomController(sessionMock.Object, roomRepositoryMock.Object, integrationEventLogServiceMock.Object, eventBusMock.Object);
    }

    [Fact]
    public async void NullData_ShouldReturnBadRequest()
    {
      // Arrange
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.UpdateAsync(null);

      // Assert
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
      Assert.Equal("Room data is null", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void EmptyName_ShouldReturnBadRequest()
    {
      // Arrange
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.UpdateAsync(new RoomDTO { Id = 1, Name = "" });

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
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.UpdateAsync(new RoomDTO { Id = 1, Name = "test" });

      // Assert
      var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(notFoundObjectResult.Value);
      Assert.Equal("Room does not exist", ((ErrorDTO)notFoundObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void OtherRoomWithPassedNameExists_ShouldReturnBadRequest()
    {
      // Arrange
      roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult(new Room() { Id = 1, Name = "test" }));
      IQueryable<Room> rooms = (new List<Room> { new Room() { Id = 2, Name = "test" } }).AsQueryable();
      roomRepositoryMock.Setup(x => x.GetAll()).Returns(rooms);
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.UpdateAsync(new RoomDTO { Id = 1, Name = "test" });

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
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.UpdateAsync(new RoomDTO { Id = 1, Name = "test" });

      // Assert
      Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async void RoomNameChanged_ShouldSaveRoomNameChangedEventToPublish()
    {
      // Arrange
      roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult(new Room() { Id = 1, Name = "test" }));
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.UpdateAsync(new RoomDTO { Id = 1, Name = "test.1" });

      // Assert
      integrationEventLogServiceMock.Verify(x => x.SaveEventAsync(It.IsAny<IntegrationEvent>()), Times.Exactly(1));
    }

    [Fact]
    public async void RoomNameUnchanged_ShouldNotSaveRoomNameChangedEventToPublish()
    {
      // Arrange
      roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult(new Room() { Id = 1, Name = "test" }));
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.UpdateAsync(new RoomDTO { Id = 1, Name = "test" });

      // Assert
      integrationEventLogServiceMock.Verify(x => x.SaveEventAsync(It.IsAny<IntegrationEvent>()), Times.Never);
    }
  }
}
