using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Models
{
  public class CancelSliceRequest
  {
    public long? OrderId { get; set; }

    public long? PurchaserId { get; set; }
  }
}
