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
  public class RoomApiClient : IRoomApiClient
  {
    private readonly HttpClient apiClient;
    private readonly ILogger<RoomApiClient> logger;
    private readonly UrlsConfig urls;

    public RoomApiClient(
      HttpClient httpClient,
      ILogger<RoomApiClient> logger,
      IOptions<UrlsConfig> config)
    {
      apiClient = httpClient;
      this.logger = logger;
      urls = config.Value;
    }

    public async Task<RoomDTO> GetAsync(long? roomId)
    {
      var response = await apiClient.GetAsync(urls.Room + UrlsConfig.RoomOperations.GetById(roomId.Value));

      response.EnsureSuccessStatusCode();

      var roomsResponse = await response.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<RoomDTO>(roomsResponse);
    }
  }
}
