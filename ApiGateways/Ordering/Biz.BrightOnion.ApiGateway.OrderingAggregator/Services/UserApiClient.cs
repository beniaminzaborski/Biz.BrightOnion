using Biz.BrightOnion.ApiGateway.OrderingAggregator.Models;
using Microsoft.Extensions.Logging;
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
    // private readonly UrlsConfig _urls;

    public UserApiClient(
      HttpClient httpClient,
      ILogger<UserApiClient> logger)
    {
      apiClient = httpClient;
      this.logger = logger;
    }

    public async Task<IEnumerable<UserDTO>> GetAllAsync()
    {

    }
  }
}
