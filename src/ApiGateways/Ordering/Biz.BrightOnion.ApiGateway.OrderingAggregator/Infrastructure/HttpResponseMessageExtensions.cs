using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Infrastructure
{
  public static class HttpResponseMessageExtensions
  {
    public static async Task<HttpResponseMessage> CustomEnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
    {
      if(response.IsSuccessStatusCode)
        return response;

      if (response.StatusCode == HttpStatusCode.BadRequest)
      {
        var responseContent = await response.Content.ReadAsStringAsync();
        var error = JsonConvert.DeserializeObject<HttpResponseError>(responseContent);
        throw new Exception(error.ErrorMessage);
      }

      return response.EnsureSuccessStatusCode();
    }
  }

  public class HttpResponseError
  {
    public string ErrorMessage { get; set; }
  }
}
