﻿using Biz.BrightOnion.Ordering.API.Application.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Commands
{
  public class PurchaseSliceCommand : IRequest<OrderDTO>
  {
    public long? RoomId { get; private set; }

    public long? PurchaserId { get; private set; }

    public int? Quantity { get; private set; }

    public PurchaseSliceCommand(
      long? roomId,
      long? purchaserId,
      int? quantity)
    {
      this.RoomId = roomId;
      this.PurchaserId = purchaserId;
      this.Quantity = quantity;
    }
  }
}
