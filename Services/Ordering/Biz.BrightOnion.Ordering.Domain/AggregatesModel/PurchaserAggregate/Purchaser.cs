using Biz.BrightOnion.Ordering.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Domain.AggregatesModel.PurchaserAggregate
{
  public class Purchaser : Entity, IAggregateRoot
  {
    public string Email { get; private set; }
    public bool NotificationEnabled { get; private set; } = false;

    protected Purchaser() { }

    public Purchaser(
      long id,
      string email,
      bool notificationEnabled = false) : this()
    {
      this.Id = id;
      this.Email = email;
      this.NotificationEnabled = notificationEnabled;
    }

    public void SetNotificationEnabled(bool notificationEnabled)
    {
      this.NotificationEnabled = notificationEnabled;
    }
  }
}
