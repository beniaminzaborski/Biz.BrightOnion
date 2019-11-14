using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Models
{
  public class RoomDTO
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public long? ManagerId { get; set; }
    public int SlicesPerPizza { get; set; }
  }
}
