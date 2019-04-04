using System;
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
      public static string CancelSlice() => "/api/v1/orders/cancel";
      public static string ApproveOrder() => "/api/v1/orders/{0}/approve";
    }

    public string Room { get; set; }
    public string User { get; set; }
    public string Order { get; set; }
  }
}
