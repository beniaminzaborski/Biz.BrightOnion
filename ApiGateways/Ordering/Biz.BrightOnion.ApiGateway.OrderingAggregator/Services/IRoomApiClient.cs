using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Services
{
  public interface IRoomApiClient
  {
    Task<string> GetNameAsync(long? roomId);
  }
}
