using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ServicesRegistry
{
    public interface IServiceRegistry
    {
        Task<ServiceInfo> GetAsync(string name);
    }
}
