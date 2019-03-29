using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Commands
{
  public class ApproveOrderCommand : IRequest<bool>
  {
    public long? OrderId { get; private set; }

    public ApproveOrderCommand(long? orderId)
    {
      this.OrderId = orderId;
    }
  }
}
