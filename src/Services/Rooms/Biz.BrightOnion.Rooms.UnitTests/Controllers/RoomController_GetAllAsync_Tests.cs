using Biz.BrightOnion.Rooms.API.Controllers;
using Biz.BrightOnion.Rooms.API.Dto;
using Biz.BrightOnion.Rooms.API.Repositories;
using Biz.BrightOnion.Rooms.API.Services;
using Biz.BrightOnion.EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHibernate;
using System.Collections.Generic;
using Xunit;

namespace Biz.BrightOnion.Rooms.UnitTests.Controller
{
    public class RoomController_GetAllAsync_Tests
  {
    private readonly Mock<ISession> sessionMock;
    private readonly Mock<IRoomRepository> roomRepositoryMock;
    private readonly Mock<IIntegrationEventLogService> integrationEventLogServiceMock;

    public RoomController_GetAllAsync_Tests()
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
    public async void ShouldReturnActionResultWithRoomDtoList()
    {
      // Arrange
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.GetAllAsync();

      // Assert
      Assert.IsType<ActionResult<List<RoomDTO>>>(actionResult);
    }
  }
}
