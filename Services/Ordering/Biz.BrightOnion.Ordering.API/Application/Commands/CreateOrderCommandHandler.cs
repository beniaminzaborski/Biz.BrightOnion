using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Commands
{
  public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDTO>
  {
    private readonly IOrderRepository orderRepository;

    public CreateOrderCommandHandler(
      IOrderRepository orderRepository)
    {
      this.orderRepository = orderRepository;
    }

    public async Task<OrderDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
      DateTime day = DateTime.Now.Date;

      Order order = await orderRepository.GetByDayEagerAsync(day);
      bool orderExists = order != null;

      if (!orderExists)
        order = new Order(request.RoomId, day);

      order.AddOrderItem(request.PurchaserId, request.Quantity);

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

    public static OrderDTO FromOrder(Order order)
    {
      return new OrderDTO()
      {
        OrderId = order.Id,
        RoomId = order.RoomId,
        Day = order.Day
      };
    }
  }
}
