using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biz.BrightOnion.ApiGateway.OrderingAggregator.Models;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Services
{
  public interface IOrderApiClient
  {
    Task<OrderDTO> PurchaseSliceAsync(long? roomId, string roomName, long? purchaserId, int? quantity);

    Task<OrderDTO> CancelSliceAsync(long? orderId, long? purchaserId);
  }
}
