using Biz.BrightOnion.ApiGateway.OrderingAggregator.Config;
using Biz.BrightOnion.ApiGateway.OrderingAggregator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
    private readonly UrlsConfig urls;

    public OrderApiClient(
      HttpClient httpClient, 
      ILogger<OrderApiClient> logger,
      IOptions<UrlsConfig> config)
    {
      apiClient = httpClient;
      this.logger = logger;
      urls = config.Value;
    }

    public async Task<OrderDTO> PurchaseSliceAsync(long? roomId, string roomName, long? purchaserId, int? quantity)
    {
      var requestData = new { RoomId = roomId, RoomName = roomName, PurchaserId = purchaserId, Quantity = quantity };
      var requestDataContent = new StringContent(JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json");

      var response = await apiClient.PostAsync(urls.Order + UrlsConfig.OrderOperations.PurchaseSlice(), requestDataContent);

      response.EnsureSuccessStatusCode();

      var orderResponse = await response.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<OrderDTO>(orderResponse);
    }

    public async Task<OrderDTO> CancelSliceAsync(long? orderId, long? purchaserId)
    {
      var requestData = new { OrderId = orderId, PurchaserId = purchaserId };
      var requestDataContent = new StringContent(JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json");

      var response = await apiClient.PostAsync(urls.Order + UrlsConfig.OrderOperations.CancelSlice(), requestDataContent);

      response.EnsureSuccessStatusCode();

      var orderResponse = await response.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<OrderDTO>(orderResponse);
    }
  }
}
