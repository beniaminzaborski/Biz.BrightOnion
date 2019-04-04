using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Models
{
  public class ApproveOrderRequest
  {
    public long? OrderId { get; set; }

    public long? RoomManagerId { get; set; }
  }
}
