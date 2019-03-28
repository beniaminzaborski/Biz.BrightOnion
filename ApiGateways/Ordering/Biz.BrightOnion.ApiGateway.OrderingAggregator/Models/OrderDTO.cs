using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Models
{
  public class OrderDTO
  {
    public long OrderId { get; set; }
    public long RoomId { get; set; }
    public DateTime Day { get; set; }
    public IList<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    public int TotalPizzas { get; set; }
    public int FreeSlicesToGrab { get; set; }
  }

  public class OrderItemDTO
  {
    public long OrderItemId { get; set; }
    public long PurchaserId { get; set; }
    public int Quantity { get; set; }
  }
}
