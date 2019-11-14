using Biz.BrightOnion.Ordering.Domain.AggregatesModel.PurchaserAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Commands
{
  public class CreatePurchaserCommandHandler : IRequestHandler<CreatePurchaserCommand>
  {
    private readonly IPurchaserRepository purchaserRepository;

    public CreatePurchaserCommandHandler(IPurchaserRepository purchaserRepository)
    {
      this.purchaserRepository = purchaserRepository;
    }

    public async Task<Unit> Handle(CreatePurchaserCommand request, CancellationToken cancellationToken)
    {
      bool exists = await purchaserRepository.CheckIfExistsAsync(request.PurchaserId.Value);
      if (!exists)
      {
        var purchaser = new Purchaser(request.PurchaserId.Value, request.Email);
        purchaserRepository.Add(purchaser);
        await purchaserRepository.UnitOfWork.SaveChangesAsync();
      }

      return Unit.Value;
    }
  }
}
