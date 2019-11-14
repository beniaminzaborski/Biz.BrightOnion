using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Commands
{
  public class CreatePurchaserCommand : IRequest
  {
    public long? PurchaserId { get; private set; }

    public string Email { get; private set; }

    public CreatePurchaserCommand(
      long? purchaserId,
      string email)
    {
      this.PurchaserId = purchaserId;
      this.Email = email;
    }
  }
}
