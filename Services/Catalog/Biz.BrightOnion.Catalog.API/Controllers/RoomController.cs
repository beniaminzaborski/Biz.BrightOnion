using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biz.BrightOnion.Catalog.API.Dto;
using Biz.BrightOnion.Catalog.API.Entities;
using Biz.BrightOnion.Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using NHibernate;

namespace Biz.BrightOnion.Catalog.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class RoomController : ControllerBase
  {
    private readonly ISession session;
    private readonly IRoomRepository roomRepository;

    public RoomController(
      ISession session,
      IRoomRepository roomRepository)
    {
      this.session = session;
      this.roomRepository = roomRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomDTO>>> GetAllAsync()
    {
      var rooms = roomRepository.GetAll().ToList();
      IList<RoomDTO> result = new List<RoomDTO>();
      rooms.ForEach(r => result.Add(new RoomDTO { Id = r.Id, Name = r.Name }));
      return new ObjectResult(result);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> Create([FromBody]RoomDTO roomDTO)
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

      return new ObjectResult(new RoomDTO { Id = id, Name = room.Name });
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> Update([FromBody]RoomDTO roomDTO)
    {
      if (roomDTO == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Room data is null" });

      if (string.IsNullOrWhiteSpace(roomDTO.Name))
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Room name is empty" });

      var room = await roomRepository.GetByIdAsync(roomDTO.Id);
      if (room == null)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Room does not exist" });

      bool roomWithNameExists = roomRepository.GetAll().Any(r => r.Id != roomDTO.Id && r.Name == roomDTO.Name);
      if(roomWithNameExists)
        return new BadRequestObjectResult(new ErrorDTO { ErrorMessage = "Room with passed name does exist" });

      room.Name = roomDTO.Name;

      using (var transaction = session.BeginTransaction())
      {
        await roomRepository.UpdateAsync(room.Id, room);
        transaction?.Commit();
      }

      // TODO: Raise integration event: RoomUpdateEvent

      return NoContent();
    }
  }
}