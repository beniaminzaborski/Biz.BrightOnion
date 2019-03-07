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
    public CreateOrderCommandHandler()
    {

    }

    public async Task<OrderDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }

  public class OrderDTO
  {
    public long OrderId { get; set; }
    public long RoomId { get; private set; }
    public long PurchaserId { get; private set; }
    public int Quantity { get; private set; }

    //public static OrderDTO FromOrder(Order order)
    //{
    //  return new OrderDTO()
    //  {
    //    OrderId = order.Id,
    //    RoomId = order.RoomId,
    //    PurchaserId = order.PurchaserId,
    //    Quantity = order.Quantity
    //  };
    //}
  }
}
