using Biz.BrightOnion.Ordering.API.Application.Commands;
using Biz.BrightOnion.Ordering.API.Application.Dto;
using Biz.BrightOnion.Ordering.API.Application.Queries;
using Biz.BrightOnion.Ordering.API.Infrastructure.Services;
using Biz.BrightOnion.Ordering.API.Models;
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
    private readonly IIdentityService identityService;

    public OrdersController(
      IMediator mediator,
      IOrderQueries orderQueries,
      ILogger<OrdersController> logger,
      IIdentityService identityService)
    {
      this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
      this.orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
      this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    }

    [HttpGet("{roomId}")]
    [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Order>> GetOrdersInRoomAsync(long roomId)
    {
      var day = DateTime.Now.Date;
      var order = await orderQueries.GetOrderInRoomForDayAsync(roomId, day);
      return Ok(order);
    }

    [Route("make")]
    [HttpPost]
    [ProducesResponseType(typeof(OrderDTO), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<OrderDTO>> PurchaseSliceAsync([FromBody] PurchaseSliceRequest purchaseSliceRequest)
    {
      string userId = identityService.GetUserIdentity();
      return await mediator.Send(new PurchaseSliceCommand(purchaseSliceRequest.RoomId, purchaseSliceRequest.RoomName, Int64.Parse(userId), purchaseSliceRequest.Quantity));
    }

    [Route("cancel")]
    [HttpPost]
    [ProducesResponseType(typeof(OrderDTO), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<OrderDTO>> CancelSliceAsync([FromBody] CancelSliceRequest cancelSliceRequest)
    {
      string userId = identityService.GetUserIdentity();
      return await mediator.Send(new CancelSliceCommand(cancelSliceRequest.OrderId, Int64.Parse(userId)));
    }

    [Route("approve")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> ApproveOrderAsync([FromBody] ApproveOrderRequest approveOrderRequest)
    {
      string userId = identityService.GetUserIdentity();
      var result = await mediator.Send(new ApproveOrderCommand(approveOrderRequest.OrderId, approveOrderRequest.RoomManagerId, userId));
      return result ? (ActionResult)NoContent() : (ActionResult)BadRequest();
    }
  }
}
