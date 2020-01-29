using Consul;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ServicesRegistry.Consul
{
    public class ConsulServiceRegistry : IServiceRegistry
    {
        private readonly Config.Consul _consulConfig;
        private readonly IServiceLoadBalancer _loadBalancer;

        public ConsulServiceRegistry(
            IOptions<Config.Consul> consulConfig,
            IServiceLoadBalancer loadBalancer)
        {
            _consulConfig = consulConfig.Value ?? throw new ArgumentNullException("consulConfig");
            _loadBalancer = loadBalancer;
        }

        public Task<ServiceInfo> GetAsync(string name)
        {
            var consulClientConfig = new ConsulClientConfiguration
            {
                Address = new Uri(_consulConfig.Url)
            };
            
            var query = new ConsulClient(consulClientConfig).Catalog.Service(name).GetAwaiter().GetResult();
            
            var serviceInstance =_loadBalancer.GetService(query.Response.Select(s => ConvertToServiceInfo(s)));
            
            return Task.FromResult(serviceInstance);
        }

        private ServiceInfo ConvertToServiceInfo(CatalogService catalogService)
        {
            var uriBuilder = new UriBuilder();
            uriBuilder.Host = catalogService.ServiceAddress; //new Uri(catalogService.ServiceAddress).Host;
            uriBuilder.Port = catalogService.ServicePort;

            return new ServiceInfo
            {
                Address = uriBuilder.Uri
            };
        }
    }
}
