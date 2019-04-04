using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biz.BrightOnion.Catalog.API.Dto;
using Biz.BrightOnion.Catalog.API.Entities;
using Biz.BrightOnion.Catalog.API.Events;
using Biz.BrightOnion.Catalog.API.Repositories;
using Biz.BrightOnion.Catalog.API.Services;
using Biz.BrightOnion.EventBus.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHibernate;

namespace Biz.BrightOnion.Catalog.API.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class RoomController : ControllerBase
  {
    private readonly ISession session;
    private readonly IRoomRepository roomRepository;
    private readonly IIntegrationEventLogService integrationEventLogService;
    private readonly IEventBus eventBus;

    public RoomController(
      ISession session,
      IRoomRepository roomRepository,
      IIntegrationEventLogService integrationEventLogService,
      IEventBus eventBus)
    {
      this.session = session;
      this.roomRepository = roomRepository;
      this.integrationEventLogService = integrationEventLogService;
      this.eventBus = eventBus;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomDTO>>> GetAllAsync()
    {
      var rooms = roomRepository.GetAll().ToList();
      IList<RoomDTO> result = new List<RoomDTO>();
      rooms.ForEach(r => result.Add(new RoomDTO { Id = r.Id, Name = r.Name, ManagerId = r.ManagerId, ManagerName = r.ManagerName }));
      return new ObjectResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(RoomDTO), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<RoomDTO>> GetAsync(long id)
    {
      var room = await roomRepository.GetByIdAsync(id);

      if (room == null)
        return NotFound(new ErrorDTO { ErrorMessage = "Room does not exist" });

      return Ok(new RoomDTO { Id = room.Id, Name = room.Name, ManagerId = room.ManagerId, ManagerName = room.ManagerName });
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(RoomDTO), (int)HttpStatusCode.Created)]
    public async Task<ActionResult> CreateAsync([FromBody]RoomDTO roomDTO)
    {
      long id;

      if (roomDTO == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Room data is null" });

      if (string.IsNullOrWhiteSpace(roomDTO.Name))
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Room name is empty" });

      var room = await roomRepository.GetByNameAsync(roomDTO.Name);
      if (room != null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Room does exist" });

      room = new Room { Name = roomDTO.Name, ManagerId = roomDTO.ManagerId, ManagerName = roomDTO.ManagerName };

      using (var transaction = session.BeginTransaction())
      {
        id = await roomRepository.CreateAsync(room);
        transaction?.Commit();
      }

      return CreatedAtAction(nameof(GetAsync), new { id = room.Id }, new RoomDTO { Id = room.Id, Name = room.Name, ManagerId = room.ManagerId, ManagerName = room.ManagerName });
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> UpdateAsync([FromBody]RoomDTO roomDTO)
    {
      if (roomDTO == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Room data is null" });

      if (string.IsNullOrWhiteSpace(roomDTO.Name))
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Room name is empty" });

      var room = await roomRepository.GetByIdAsync(roomDTO.Id);
      if (room == null)
        // return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Room does not exist" });
        return NotFound(new ErrorDTO { ErrorMessage = "Room does not exist" });


      bool roomWithNameExists = roomRepository.GetAll().Any(r => r.Id != roomDTO.Id && r.Name == roomDTO.Name);
      if (roomWithNameExists)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Room with passed name does exist" });

      bool isRoomNameChanged = false, isRoomManagerChanged = false;
      RoomNameChangedEvent roomNameChangedEvent = null;
      RoomManagerChangedEvent roomManagerChangedEvent = null;

      if (room.Name != roomDTO.Name)
      {
        isRoomNameChanged = true;

        room.Name = roomDTO.Name;

        roomNameChangedEvent = new RoomNameChangedEvent(room.Id, room.Name);
      }

      if (room.ManagerId != roomDTO.ManagerId)
      {
        isRoomManagerChanged = true;

        room.ManagerId = roomDTO.ManagerId;
        room.ManagerName = roomDTO.ManagerName;

        roomManagerChangedEvent = new RoomManagerChangedEvent(room.Id, room.Name, room.ManagerId, room.ManagerName);
      }

      if (isRoomNameChanged || isRoomManagerChanged)
      {
        // Publish integration events
        using (var transaction = session.BeginTransaction())
        {
          await roomRepository.UpdateAsync(room.Id, room);
          if(isRoomNameChanged)
            await integrationEventLogService.SaveEventAsync(roomNameChangedEvent);
          if (isRoomManagerChanged)
            await integrationEventLogService.SaveEventAsync(roomManagerChangedEvent);
          transaction?.Commit();
        }
      }

      return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeleteAsync(long id)
    {
      var room = await roomRepository.GetByIdAsync(id);
      if (room == null)
        return NotFound(new ErrorDTO { ErrorMessage = "Room does not exist" });

      var roomDeletedEvent = new RoomDeletedEvent(room.Id);

      using (var transaction = session.BeginTransaction())
      {
        await roomRepository.DeleteAsync(id);
        await integrationEventLogService.SaveEventAsync(roomDeletedEvent);
        transaction?.Commit();
      }

      // Publish integration event: RoomDeletedEvent
      using (var transaction = session.BeginTransaction())
      {
        eventBus.Publish(roomDeletedEvent);
        await integrationEventLogService.MarkEventAsPublishedAsync(roomDeletedEvent.EventId);
        transaction?.Commit();
      }

      return NoContent();
    }
  }
}