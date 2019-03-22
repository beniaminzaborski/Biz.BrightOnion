using Biz.BrightOnion.Ordering.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Domain.AggregatesModel.PurchaserAggregate
{
  public class Purchaser : Entity, IAggregateRoot
  {
    public string Email { get; private set; }
    public bool NotificationEnabled { get; private set; }

    protected Purchaser() { }

    public Purchaser(
      long id,
      string email) : this()
    {
      this.Id = id;
      this.Email = email;
    }
  }
}
