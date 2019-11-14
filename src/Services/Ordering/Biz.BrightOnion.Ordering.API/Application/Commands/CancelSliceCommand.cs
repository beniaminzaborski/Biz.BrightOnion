using Biz.BrightOnion.Ordering.API.Application.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Commands
{
  public class CancelSliceCommand : IRequest<OrderDTO>
  {
    public long? OrderId { get; private set; }

    public long? PurchaserId { get; private set; }

    public CancelSliceCommand(
      long? orderId,
      long? purchaserId)
    {
      this.OrderId = orderId;
      this.PurchaserId = purchaserId;
    }
  }
}
