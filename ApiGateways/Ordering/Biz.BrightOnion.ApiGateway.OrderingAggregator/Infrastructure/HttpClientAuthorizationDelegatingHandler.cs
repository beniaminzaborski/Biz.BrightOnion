using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Infrastructure
{
  public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
  {
    private readonly IHttpContextAccessor httpContextAccesor;

    public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccesor)
    {
      this.httpContextAccesor = httpContextAccesor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var authorizationHeader = httpContextAccesor.HttpContext
          .Request.Headers["Authorization"];

      if (!string.IsNullOrEmpty(authorizationHeader))
      {
        request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
      }

      //var token = await GetToken();

      //if (token != null)
      //{
      //  request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
      //}

      return await base.SendAsync(request, cancellationToken);
    }

    //async Task<string> GetToken()
    //{
    //  const string ACCESS_TOKEN = "access_token";

    //  return await httpContextAccesor.HttpContext
    //      .GetTokenAsync(ACCESS_TOKEN);
    //}
  }
}
