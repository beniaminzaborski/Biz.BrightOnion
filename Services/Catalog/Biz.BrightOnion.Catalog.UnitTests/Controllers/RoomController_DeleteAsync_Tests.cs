﻿using Biz.BrightOnion.Catalog.API.Controllers;
using Biz.BrightOnion.Catalog.API.Dto;
using Biz.BrightOnion.Catalog.API.Entities;
using Biz.BrightOnion.Catalog.API.Repositories;
using Biz.BrightOnion.Catalog.API.Services;
using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.EventBus.Events;
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
  public class RoomController_DeleteAsync_Tests
  {
    private readonly Mock<ISession> sessionMock;
    private readonly Mock<IRoomRepository> roomRepositoryMock;
    private readonly Mock<IIntegrationEventLogService> integrationEventLogServiceMock;
    private readonly Mock<IEventBus> eventBusMock;

    public RoomController_DeleteAsync_Tests()
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
    public async void RoomDoesNotExist_ShouldReturnBadRequest()
    {
      // Arrange
      roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult((Room)null));
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.DeleteAsync(1);

      // Assert
      var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(actionResult);
      Assert.IsAssignableFrom<ErrorDTO>(notFoundObjectResult.Value);
      Assert.Equal("Room does not exist", ((ErrorDTO)notFoundObjectResult.Value).ErrorMessage);
    }

    [Fact]
    public async void ShouldReturnNoContentResult()
    {
      // Arrange
      roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult(new Room() { Id = 1, Name = "test" }));
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.DeleteAsync(1);

      // Assert
      Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async void ShouldSaveRoomDeletedEventToPublish()
    {
      // Arrange
      roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult(new Room() { Id = 1, Name = "test" }));
      var roomController = CreateRoomController();

      // Act
      var actionResult = await roomController.DeleteAsync(1);

      // Assert
      integrationEventLogServiceMock.Verify(x => x.SaveEventAsync(It.IsAny<IntegrationEvent>()), Times.Exactly(1));
    }
  }
}
