using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Commands
{
  public class ApproveOrderCommandHandler : IRequestHandler<ApproveOrderCommand, bool>
  {
    private readonly IOrderRepository orderRepository;

    public ApproveOrderCommandHandler(
      IOrderRepository orderRepository)
    {
      this.orderRepository = orderRepository;
    }

    public async Task<bool> Handle(ApproveOrderCommand request, CancellationToken cancellationToken)
    {
      Order order = await orderRepository.GetAsync(request.OrderId.Value);
      if (order == null)
        return false;

      if (!order.IsApproved)
      {
        order.Approve();

        orderRepository.Update(order);

        await orderRepository.UnitOfWork
          .SaveEntitiesAsync();

        return true;
      }

      return false;
    }
  }
}
