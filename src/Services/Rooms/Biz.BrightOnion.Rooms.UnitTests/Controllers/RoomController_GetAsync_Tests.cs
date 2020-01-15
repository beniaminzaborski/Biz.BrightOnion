using Biz.BrightOnion.Rooms.API.Controllers;
using Biz.BrightOnion.Rooms.API.Dto;
using Biz.BrightOnion.Rooms.API.Entities;
using Biz.BrightOnion.Rooms.API.Repositories;
using Biz.BrightOnion.Rooms.API.Services;
using Biz.BrightOnion.EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHibernate;
using System.Threading.Tasks;
using Xunit;

namespace Biz.BrightOnion.Rooms.UnitTests.Controller
{
    public class RoomController_GetAsync_Tests
  {
    private readonly Mock<ISession> sessionMock;
    private readonly Mock<IRoomRepository> roomRepositoryMock;
    private readonly Mock<IIntegrationEventLogService> integrationEventLogServiceMock;

    public RoomController_GetAsync_Tests()
    {
      sessionMock = new Mock<ISession>();
      roomRepositoryMock = new Mock<IRoomRepository>();
      integrationEventLogServiceMock = new Mock<IIntegrationEventLogService>();
    }

    private RoomController CreateRoomController()
    {
      return new RoomController(sessionMock.Object, roomRepositoryMock.Object, integrationEventLogServiceMock.Object);
    }

    [Fact]
    public async void RoomDoesNotExist_ShouldReturnNotFound()
    {
      // Arrange
      roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult((Room)null));
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.GetAsync(1);

      // Assert
      var result = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
      var notFoundObjectResult = (NotFoundObjectResult)actionResult.Result;
      Assert.IsAssignableFrom<ErrorDTO>(notFoundObjectResult.Value);
      Assert.Equal("Room does not exist", ((ErrorDTO)notFoundObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void ShouldReturnRoomDTO()
    {
      // Arrange
      var roomDTO = new RoomDTO { Name = "test" };
      roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult(new Room { Id = 1, Name = "test" }));
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.GetAsync(1);

      // Assert
      var result = Assert.IsType<OkObjectResult>(actionResult.Result);
      Assert.IsAssignableFrom<RoomDTO>(result.Value);
    }
  }
}
