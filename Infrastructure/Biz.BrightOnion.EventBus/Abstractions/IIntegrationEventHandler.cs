using Biz.BrightOnion.EventBus.Events;
using System.Threading.Tasks;

namespace Biz.BrightOnion.EventBus.Abstractions
{
  public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
  {
    Task Handle(TIntegrationEvent @event);
  }

  public interface IIntegrationEventHandler { }
}
