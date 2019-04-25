using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biz.BrightOnion.ApiGateway.OrderingAggregator.Models;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Services
{
  public interface IUserApiClient
  {
    Task<IEnumerable<UserDTO>> GetAllAsync();
  }
}
