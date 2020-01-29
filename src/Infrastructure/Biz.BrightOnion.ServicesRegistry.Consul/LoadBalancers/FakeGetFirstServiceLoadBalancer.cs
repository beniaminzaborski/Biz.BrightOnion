using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.BrightOnion.ServicesRegistry.Consul.LoadBalancers
{
    public class FakeGetFirstServiceLoadBalancer : IServiceLoadBalancer
    {
        public ServiceInfo GetService(IEnumerable<ServiceInfo> services)
        {
            return services.First();
        }
    }
}
