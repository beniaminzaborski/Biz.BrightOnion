using Biz.BrightOnion.Ordering.API.Application.Dto;
using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
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
    // private readonly IMailerService mailerService;

    public PurchaseSliceCommandHandler(
      IOrderRepository orderRepository/*,
      IMailerService mailerService*/)
    {
      this.orderRepository = orderRepository;
      // this.mailerService = mailerService;
    }

    public async Task<OrderDTO> Handle(PurchaseSliceCommand request, CancellationToken cancellationToken)
    {
      DateTime day = DateTime.Now.Date;

      Order order = await orderRepository.GetByDayAndRoomEagerAsync(day, request.RoomId.Value);
      bool orderExists = order != null;

      if (!orderExists)
      {
        order = new Order(request.RoomId.Value, day);
        // TODO: Publish domain event - NewOrderCreatedDomainEvent.
        // TODO: Handle NewOrderCreatedDomainEvent and send e-mail with IMailerService
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
