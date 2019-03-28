using Biz.BrightOnion.ApiGateway.OrderingAggregator.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Services
{
  public class OrderApiClient : IOrderApiClient
  {
    private readonly HttpClient apiClient;
    private readonly ILogger<OrderApiClient> logger;
    // private readonly UrlsConfig _urls;

    public OrderApiClient(
      HttpClient httpClient, 
      ILogger<OrderApiClient> logger)
    {
      apiClient = httpClient;
      this.logger = logger;
    }

    public async Task<OrderDTO> PurchaseSliceAsync(long? roomId, string roomName, long? purchaserId, int? quantity)
    {

    }
  }
}
