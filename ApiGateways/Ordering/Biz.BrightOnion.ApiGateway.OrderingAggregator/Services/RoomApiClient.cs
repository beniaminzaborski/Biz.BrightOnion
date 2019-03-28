using Microsoft.Extensions.Logging;
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

    public async Task<string> GetNameAsync(long? roomId)
    {

    }
  }
}
