using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ServicesRegistry.Consul.Http
{
    public class HttpClientConsulServiceDiscoveryDelegatingHandler : DelegatingHandler
    {
        private readonly IServiceRegistry _serviceRegistry;
        private readonly Config.Consul _consulConfig;

        public HttpClientConsulServiceDiscoveryDelegatingHandler(
            IServiceRegistry servicesRegistry,
            IOptions<Config.Consul> consulConfig)
        {
            _serviceRegistry = servicesRegistry;
            _consulConfig = consulConfig.Value ?? throw new ArgumentNullException("consulConfig");
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_consulConfig.Enabled) // TODO: Get service name !!!
                request.RequestUri = await GetRequestUriAsync(request.RequestUri.Host, request.RequestUri);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<Uri> GetRequestUriAsync(string serviceName, Uri uri)
        {
            var serviceInfo = await _serviceRegistry.GetAsync(serviceName);
            
            var uriBuilder = new UriBuilder(uri)
            {
                Host = serviceInfo.Address.Host,
                Port = serviceInfo.Address.Port
            };

            return uriBuilder.Uri;
        }
    }
}
