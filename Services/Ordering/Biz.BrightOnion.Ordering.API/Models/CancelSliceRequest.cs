using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Models
{
  public class CancelSliceRequest
  {
    public long? OrderId { get; set; }
  }
}
