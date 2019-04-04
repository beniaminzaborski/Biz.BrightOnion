using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biz.BrightOnion.ApiGateway.OrderingAggregator.Models;
using Microsoft.AspNetCore.Mvc;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Services
{
  public interface IOrderApiClient
  {
    Task<OrderDTO> PurchaseSliceAsync(long? roomId, string roomName, int? quantity);

    Task<OrderDTO> CancelSliceAsync(long? orderId);

    Task<HttpStatusCode> ApproveOrderAsync(long? orderId, long? roomManagerId);
  }
}
