using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Models
{
  public class OrderData
  {
    public long OrderId { get; set; }
    public long RoomId { get; set; }
    public DateTime Day { get; set; }
    public IList<OrderItemData> OrderItems { get; set; } = new List<OrderItemData>();
    public int TotalPizzas { get; set; }
    public int FreeSlicesToGrab { get; set; }

    public static OrderData FromOrderDTO(OrderDTO orderDTO, IEnumerable<UserDTO> users)
    {
      if (orderDTO == null)
        return null;

      return new OrderData()
      {
        OrderId = orderDTO.OrderId,
        RoomId = orderDTO.RoomId,
        Day = orderDTO.Day,
        TotalPizzas = orderDTO.TotalPizzas,
        FreeSlicesToGrab = orderDTO.FreeSlicesToGrab,
        OrderItems = orderDTO.OrderItems
          .Select(i => new OrderItemData { PurchaserId = i.PurchaserId, PurchaserEmail = users.FirstOrDefault(u => u.UserId == i.PurchaserId)?.Email, Quantity = i.Quantity }).ToList()
      };
    }
  }

  public class OrderItemData
  {
    public long PurchaserId { get; set; }
    public string PurchaserEmail { get; set; }
    public int Quantity { get; set; }
  }
}
