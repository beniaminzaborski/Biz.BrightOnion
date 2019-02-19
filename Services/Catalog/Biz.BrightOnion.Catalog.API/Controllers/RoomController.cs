using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biz.BrightOnion.Catalog.API.Dto;
using Biz.BrightOnion.Catalog.API.Entities;
using Biz.BrightOnion.Catalog.API.Events;
using Biz.BrightOnion.Catalog.API.Repositories;
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
    // private readonly IEventBus eventBus;

    public RoomController(
      ISession session,
      IRoomRepository roomRepository/*,
      IEventBus eventBus*/)
    {
      this.session = session;
      this.roomRepository = roomRepository;
      // this.eventBus = eventBus;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomDTO>>> GetAllAsync()
    {
      var rooms = roomRepository.GetAll().ToList();
      IList<RoomDTO> result = new List<RoomDTO>();
      rooms.ForEach(r => result.Add(new RoomDTO { Id = r.Id, Name = r.Name }));
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

      return Ok(new RoomDTO { Id = room.Id, Name = room.Name });
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

      room = new Room { Name = roomDTO.Name };

      using (var transaction = session.BeginTransaction())
      {
        id = await roomRepository.CreateAsync(room);
        transaction?.Commit();
      }

      return CreatedAtAction(nameof(GetAsync), new { id = room.Id }, new RoomDTO { Id = room.Id, Name = room.Name });
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

      if (room.Name != roomDTO.Name)
      {
        room.Name = roomDTO.Name;

        using (var transaction = session.BeginTransaction())
        {
          await roomRepository.UpdateAsync(room.Id, room);
          transaction?.Commit();
        }

        // TODO: Raise integration event: RoomNameChangedEvent
        // eventBus.Publish(new RoomNameChangedEvent(room.Id, room.Name));
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

      using (var transaction = session.BeginTransaction())
      {
        await roomRepository.DeleteAsync(id);
        transaction?.Commit();
      }

      // TODO: Raise integration event: RoomDeletedEvent
      // eventBus.Publish(new RoomDeletedEvent(room.Id));

      return NoContent();
    }
  }
}