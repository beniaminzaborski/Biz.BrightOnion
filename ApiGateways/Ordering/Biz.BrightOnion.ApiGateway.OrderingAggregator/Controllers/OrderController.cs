﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biz.BrightOnion.ApiGateway.OrderingAggregator.Models;
using Biz.BrightOnion.ApiGateway.OrderingAggregator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Controllers
{
  [Route("api/[controller]")]
  // [Authorize]
  [ApiController]
  public class OrderController : ControllerBase
  {
    private readonly IUserApiClient userClient;
    private readonly IRoomApiClient roomApiClient;
    private readonly IOrderApiClient orderClient;

    public OrderController(
      IUserApiClient userClient,
      IRoomApiClient roomApiClient,
      IOrderApiClient orderClient)
    {
      this.userClient = userClient;
      this.roomApiClient = roomApiClient;
      this.orderClient = orderClient;
    }

    [Route("make")]
    [HttpPost]
    [ProducesResponseType(typeof(OrderData), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<OrderData>> PurchaseSliceAsync([FromBody] PurchaseSliceRequest data)
    {
      // Step 1: Get room by data.RoomId
      var room = await roomApiClient.GetAsync(data.RoomId);

      // Step 2: Call purchase order on orderClient
      var orderDTO = await orderClient.PurchaseSliceAsync(data.RoomId, room.Name, data.PurchaserId, data.Quantity);

      // Step 3: Get users list and set user's e-mail
      IEnumerable<UserDTO> users = await userClient.GetAllAsync();
      var result = OrderData.FromOrderDTO(orderDTO, users);

      return new OkObjectResult(result);
    }
  }
}
