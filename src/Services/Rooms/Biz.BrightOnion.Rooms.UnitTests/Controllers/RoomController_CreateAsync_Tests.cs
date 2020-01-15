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
    public class RoomController_CreateAsync_Tests
    {
        private readonly Mock<ISession> sessionMock;
        private readonly Mock<IRoomRepository> roomRepositoryMock;
        private readonly Mock<IIntegrationEventLogService> integrationEventLogServiceMock;

        public RoomController_CreateAsync_Tests()
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
        public async void NullData_ShouldReturnBadRequest()
        {
            // Arrange
            var roomController = CreateRoomController();

            // Act
            var actionResult = await roomController.CreateAsync(null);

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
            var actionResult = await roomController.CreateAsync(new RoomDTO { Name = "" });

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

            var roomController = CreateRoomController();

            // Act
            var actionResult = await roomController.CreateAsync(new RoomDTO { Name = "test" });

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.IsAssignableFrom<ErrorDTO>(badRequestObjectResult.Value);
            Assert.Equal("Room does exist", ((ErrorDTO)badRequestObjectResult.Value).ErrorMessage);
        }

        [Fact]
        public async void ShouldReturnCreated()
        {
            // Arrange
            var roomDTO = new RoomDTO { Name = "test" };

            var roomController = CreateRoomController();

            // Act
            var actionResult = await roomController.CreateAsync(roomDTO);

            // Assert
            var result = Assert.IsType<CreatedAtActionResult>(actionResult);
            Assert.IsAssignableFrom<RoomDTO>(result.Value);
            //var objectResult = Assert.IsType<ObjectResult>(actionResult);
            //Assert.IsAssignableFrom<RoomDTO>(objectResult.Value);
        }
    }
}
