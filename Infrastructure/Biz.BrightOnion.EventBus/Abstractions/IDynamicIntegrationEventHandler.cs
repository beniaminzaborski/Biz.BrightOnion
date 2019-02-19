using System.Threading.Tasks;

namespace Biz.BrightOnion.EventBus.Abstractions
{
  public interface IDynamicIntegrationEventHandler
  {
    Task Handle(dynamic eventData);
  }
}
