using Biz.BrightOnion.Ordering.API.Application.Commands;
using Biz.BrightOnion.Ordering.API.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Controllers
{
  // [Authorize]
  [ApiController]
  [Route("api/v1/[controller]")]
  public class OrdersController : ControllerBase
  {
    private readonly IMediator mediator;
    private readonly IOrderQueries orderQueries;
    private readonly ILogger<OrdersController> logger;

    public OrdersController(
      IMediator mediator,
      IOrderQueries orderQueries,
      ILogger<OrdersController> logger)
    {
      this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
      this.orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{roomId}")]
    [ProducesResponseType(typeof(IEnumerable<Order>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersInRoomAsync(long roomId)
    {
      var day = DateTime.Now.Date;
      var orders = await orderQueries.GetOrderItemsInRoomForDayAsync(roomId, day);
      return Ok(orders);
    }

    [Route("make")]
    [HttpPost]
    public async Task<ActionResult<OrderDTO>> PurchaseSliceAsync([FromBody] PurchaseSliceCommand purchaseSliceCommand)
    {
      return await mediator.Send(purchaseSliceCommand);
    }
  }
}
