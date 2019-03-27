using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Domain.Events
{
  public class NewOrderCreatedDomainEvent : INotification
  {
    public string Email { get; private set; }

    public NewOrderCreatedDomainEvent(string email)
    {
      this.Email = email;
    }
  }
}
