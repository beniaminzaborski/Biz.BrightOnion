using Biz.BrightOnion.Ordering.API.Application.Dto;
using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using Biz.BrightOnion.Ordering.Domain.Events;
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

      var sliceCanceledDomainEvent = new SliceCanceledDomainEvent(order.Id, order.RoomId, order.Day, request.PurchaserId.Value);
      order.AddDomainEvent(sliceCanceledDomainEvent);

      if (order.OrderItems.Count() > 0)
        orderRepository.Update(order);
      else
      {
        orderRepository.Remove(order);
        order = null;
      }

      await orderRepository.UnitOfWork
        .SaveEntitiesAsync();

      return OrderDTO.FromOrder(order);
    }
  }
}
