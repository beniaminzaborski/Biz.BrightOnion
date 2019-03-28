using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Models
{
  public class PurchaseSliceRequest
  {
    public long? RoomId { get; set; }

    public long? PurchaserId { get; set; }

    public int? Quantity { get; set; }
  }
}
