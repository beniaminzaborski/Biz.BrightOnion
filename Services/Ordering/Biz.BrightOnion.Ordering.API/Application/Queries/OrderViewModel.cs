﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Queries
{
  public class Order
  {
    public long OrderId { get; set; }

    public DateTime Day { get; set; }

    public long RoomId { get; set; }

    public long PurchaserId { get; set; }

    public int Quantity { get; set; }
  }
}