using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Commands
{
  public class PurchaseSliceCommandHandler : IRequestHandler<PurchaseSliceCommand, OrderDTO>
  {
    private readonly IOrderRepository orderRepository;

    public PurchaseSliceCommandHandler(
      IOrderRepository orderRepository)
    {
      this.orderRepository = orderRepository;
    }

    public async Task<OrderDTO> Handle(PurchaseSliceCommand request, CancellationToken cancellationToken)
    {
      DateTime day = DateTime.Now.Date;

      Order order = await orderRepository.GetByDayEagerAsync(day);
      bool orderExists = order != null;

      if (!orderExists)
        order = new Order(request.RoomId.Value, day);

      order.AddOrderItem(request.PurchaserId.Value, request.Quantity.Value);

      if (!orderExists)
        order = orderRepository.Add(order);
      else
        orderRepository.Update(order);

      await orderRepository.UnitOfWork
        .SaveEntitiesAsync();

      return OrderDTO.FromOrder(order);
    }
  }

  public class OrderDTO
  {
    public long OrderId { get; set; }
    public long RoomId { get; set; }
    public DateTime Day { get; set; }
    public IList<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    public int TotalPizzas { get; set; }
    public int FreeSlicesToGrab { get; set; }

    public static OrderDTO FromOrder(Order order)
    {
      return new OrderDTO()
      {
        OrderId = order.Id,
        RoomId = order.RoomId,
        Day = order.Day,
        TotalPizzas = order.TotalPizzas,
        FreeSlicesToGrab = order.FreeSlicesToGrab,
        OrderItems = order.OrderItems.Select(i => new OrderItemDTO { OrderItemId = i.Id, PurchaserId = i.PurchaserId, Quantity = i.Quantity }).ToList()
      };
    }
  }

  public class OrderItemDTO
  {
    public long OrderItemId { get; set; }
    public long PurchaserId { get; set; }
    public int Quantity { get; set; }
  }
}
