using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.Ordering.API.Events;
using Biz.BrightOnion.Ordering.Domain.AggregatesModel.PurchaserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.EventHandlers
{
  public class UserRegisteredEventHandler : IIntegrationEventHandler<UserRegisteredEvent>
  {
    private readonly IPurchaserRepository purchaserRepository;

    public UserRegisteredEventHandler(
      IPurchaserRepository purchaserRepository)
    {
      this.purchaserRepository = purchaserRepository;
    }

    public async Task Handle(UserRegisteredEvent @event)
    {
      Console.WriteLine("Handle UserRegisteredEvent: {0}", @event.Email);

      bool exists = await purchaserRepository.CheckIfExistsAsync(@event.Id);
      if (!exists)
      {
        var purchaser = new Purchaser(@event.Id, @event.Email);
        purchaserRepository.Add(purchaser);
        await purchaserRepository.UnitOfWork.SaveChangesAsync();
      }
    }
  }
}
