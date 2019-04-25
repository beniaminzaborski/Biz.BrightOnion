using Biz.BrightOnion.Ordering.Domain.AggregatesModel.PurchaserAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Commands
{
  public class ChangePurchaserNotificationCommandHandler : IRequestHandler<ChangePurchaserNotificationCommand>
  {
    private readonly IPurchaserRepository purchaserRepository;

    public ChangePurchaserNotificationCommandHandler(IPurchaserRepository purchaserRepository)
    {
      this.purchaserRepository = purchaserRepository;
    }

    public async Task<Unit> Handle(ChangePurchaserNotificationCommand request, CancellationToken cancellationToken)
    {
      Purchaser purchaser = await purchaserRepository.Get(request.PurchaserId.Value);
      if (purchaser != null)
      {
        purchaser.SetNotificationEnabled(request.NotificationEnabled);
        purchaserRepository.Update(purchaser);
        await purchaserRepository.UnitOfWork.SaveChangesAsync();
      }

      return Unit.Value;
    }
  }
}
