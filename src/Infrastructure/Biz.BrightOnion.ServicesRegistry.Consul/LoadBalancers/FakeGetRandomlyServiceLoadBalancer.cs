using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.BrightOnion.ServicesRegistry.Consul.LoadBalancers
{
    public class FakeGetRandomlyServiceLoadBalancer : IServiceLoadBalancer
    {
        public ServiceInfo GetService(IEnumerable<ServiceInfo> services)
        {
            if (services == null || services?.Count() == 0)
                throw new ArgumentNullException("services");

            var index = new Random().Next(0, services.Count() - 1);

            return services.ToList()[index];
        }
    }
}
