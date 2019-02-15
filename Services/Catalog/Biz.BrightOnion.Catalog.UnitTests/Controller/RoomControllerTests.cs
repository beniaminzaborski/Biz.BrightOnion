﻿using Biz.BrightOnion.Catalog.API.Controllers;
using Biz.BrightOnion.Catalog.API.Dto;
using Biz.BrightOnion.Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;
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
      var accountController = new RoomController(sessionMock.Object, roomRepositoryMock.Object);

      // Act
      var actionResult = await accountController.GetAllAsync();

      // Assert
      Assert.IsType<ActionResult<List<RoomDTO>>>(actionResult);
    }
  }
}
