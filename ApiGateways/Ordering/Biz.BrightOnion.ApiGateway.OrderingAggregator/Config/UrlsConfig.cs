﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Config
{
  public class UrlsConfig
  {
    public class RoomOperations
    {
      public static string GetById(long id) => $"/api/room/{id}";
    }

    public class UserOperations
    {
      public static string GetAll() => "/api/account";
    }

    public class OrderOperations
    {
      public static string PurchaseSlice() => "/api/v1/orders/make";
    }

    public string Room { get; set; }
    public string User { get; set; }
    public string Order { get; set; }
  }
}
