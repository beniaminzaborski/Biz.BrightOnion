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
  public class UserApiClient : IUserApiClient
  {
    private readonly HttpClient apiClient;
    private readonly ILogger<UserApiClient> logger;
    private readonly UrlsConfig urls;

    public UserApiClient(
      HttpClient httpClient,
      ILogger<UserApiClient> logger,
      IOptions<UrlsConfig> config)
    {
      apiClient = httpClient;
      this.logger = logger;
      urls = config.Value;
    }

    public async Task<IEnumerable<UserDTO>> GetAllAsync()
    {
      var response = await apiClient.GetAsync(urls.User + UrlsConfig.UserOperations.GetAll());

      response.EnsureSuccessStatusCode();

      var roomsResponse = await response.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<IEnumerable<UserDTO>>(roomsResponse);
    }
  }
}
