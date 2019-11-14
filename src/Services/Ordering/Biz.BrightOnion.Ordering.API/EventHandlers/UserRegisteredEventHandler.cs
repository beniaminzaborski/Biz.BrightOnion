using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.Ordering.API.Application.Commands;
using Biz.BrightOnion.Ordering.API.Events;
using Biz.BrightOnion.Ordering.Domain.AggregatesModel.PurchaserAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.EventHandlers
{
  public class UserRegisteredEventHandler : IIntegrationEventHandler<UserRegisteredEvent>
  {
    private readonly IMediator mediator;

    public UserRegisteredEventHandler(IMediator mediator)
    {
      this.mediator = mediator;
    }

    public async Task Handle(UserRegisteredEvent @event)
    {
      Console.WriteLine("Handle UserRegisteredEvent: {0}", @event.Email);

      var createPurchaserCommand = new CreatePurchaserCommand(@event.Id, @event.Email);

      await mediator.Send(createPurchaserCommand);
    }
  }
}
