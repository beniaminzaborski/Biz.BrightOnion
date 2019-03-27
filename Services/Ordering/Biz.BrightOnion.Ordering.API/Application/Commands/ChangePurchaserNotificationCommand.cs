using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Commands
{
  public class ChangePurchaserNotificationCommand : IRequest
  {
    public long? PurchaserId { get; private set; }

    public bool NotificationEnabled { get; private set; }

    public ChangePurchaserNotificationCommand(
      long? purchaserId,
      bool notificationEnabled)
    {
      this.PurchaserId = purchaserId;
      this.NotificationEnabled = notificationEnabled;
    }
  }
}
