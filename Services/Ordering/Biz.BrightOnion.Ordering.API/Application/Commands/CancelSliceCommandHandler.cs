using Biz.BrightOnion.Ordering.API.Application.Dto;
using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Commands
{
  public class CancelSliceCommandHandler : IRequestHandler<CancelSliceCommand, OrderDTO>
  {
    private readonly IOrderRepository orderRepository;

    public CancelSliceCommandHandler(
      IOrderRepository orderRepository)
    {
      this.orderRepository = orderRepository;
    }

    public async Task<OrderDTO> Handle(CancelSliceCommand request, CancellationToken cancellationToken)
    {
      DateTime day = DateTime.Now.Date;

      Order order = await orderRepository.GetAsync(request.OrderId.Value);
      bool orderExists = order != null;

      if (!orderExists)
        return null;

      order.RemoveOrderItem(request.PurchaserId.Value);
      //if (order.OrderItems.Any())
        orderRepository.Update(order);
      //else
      //  orderRepository.Remove(order);

      await orderRepository.UnitOfWork
        .SaveEntitiesAsync();

      return OrderDTO.FromOrder(order);
    }
  }
}
