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
    public long? RoomManagerId { get; private set; }
    public string UserId { get; private set; }

    public ApproveOrderCommand(long? orderId, long? roomManagerId, string userId)
    {
      this.OrderId = orderId;
      this.RoomManagerId = roomManagerId;
      this.UserId = userId;
    }
  }
}
