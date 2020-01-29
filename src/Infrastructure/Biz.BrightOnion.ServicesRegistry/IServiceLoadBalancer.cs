using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.ServicesRegistry
{
    public interface IServiceLoadBalancer
    {
        ServiceInfo GetService(IEnumerable<ServiceInfo> services);
    }
}
