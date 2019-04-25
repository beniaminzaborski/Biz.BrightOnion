using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Models
{
  public class PurchaseSliceRequest
  {
    public long? RoomId { get; set; }

    public string RoomName { get; set; }

    public int SlicesPerPizza { get; set; }

    public int? Quantity { get; set; }
  }
}
