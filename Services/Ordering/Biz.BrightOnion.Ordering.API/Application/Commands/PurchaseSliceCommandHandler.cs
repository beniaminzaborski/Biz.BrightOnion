using Biz.BrightOnion.Ordering.API.Application.Dto;
using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using Biz.BrightOnion.Ordering.Domain.Events;
using Biz.BrightOnion.Ordering.Domain.Services;
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

    public PurchaseSliceCommandHandler(IOrderRepository orderRepository)
    {
      this.orderRepository = orderRepository;
    }

    public async Task<OrderDTO> Handle(PurchaseSliceCommand request, CancellationToken cancellationToken)
    {
      DateTime day = DateTime.Now.Date;

      Order order = await orderRepository.GetByDayAndRoomEagerAsync(day, request.RoomId.Value);
      bool orderExists = order != null;

      if (!orderExists)
      {
        order = new Order(request.RoomId.Value, request.SlicesPerPizza, day);
        var newOrderCreatedDomainEvent = new NewOrderCreatedDomainEvent(order.Id, order.Day, order.RoomId, request.RoomName, request.PurchaserId.Value);
        order.AddDomainEvent(newOrderCreatedDomainEvent);
      }
      else
      {
        order.SetSlicesPerPizza(request.SlicesPerPizza);
        var slicePurchasedDomainEvent = new SlicePurchasedDomainEvent(order.Id, order.Day, order.RoomId, request.PurchaserId.Value, request.Quantity.Value);
        order.AddDomainEvent(slicePurchasedDomainEvent);
      }

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
}
