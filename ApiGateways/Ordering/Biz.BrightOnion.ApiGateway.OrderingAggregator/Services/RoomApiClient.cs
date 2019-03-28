using Biz.BrightOnion.ApiGateway.OrderingAggregator.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Services
{
  public class RoomApiClient : IRoomApiClient
  {
    private readonly HttpClient apiClient;
    private readonly ILogger<RoomApiClient> logger;
    // private readonly UrlsConfig _urls;

    public RoomApiClient(
      HttpClient httpClient,
      ILogger<RoomApiClient> logger)
    {
      apiClient = httpClient;
      this.logger = logger;
    }

    public async Task<RoomDTO> GetAsync(long? roomId)
    {
      var response = await apiClient.GetAsync($"http://localhost:7002/api/room/{roomId}");

      response.EnsureSuccessStatusCode();

      var roomsResponse = await response.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<RoomDTO>(roomsResponse);
    }
  }
}
