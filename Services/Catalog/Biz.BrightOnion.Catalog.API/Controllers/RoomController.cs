using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biz.BrightOnion.Catalog.API.Dto;
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
      throw new NotImplementedException();
    }
  }
}