using Biz.BrightOnion.Ordering.Domain.AggregatesModel.PurchaserAggregate;
using Biz.BrightOnion.Ordering.Domain.Events;
using Biz.BrightOnion.Ordering.Domain.Services;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.DomainEventHandlers
{
  public class NotifyPurchesersWhenNewOrderCreatedDomainEventHandler : INotificationHandler<NewOrderCreatedDomainEvent>
  {
    private const string emailSubject = "Bright Onion";
    private const string emailBodyTemplate = "Oops someone is hungry. The pizza has been just opened in by {0}. Can you join me? Let's get some pizza.";

    private readonly IPurchaserRepository purchaserRepository;
    private readonly IMailerService mailerService;

    public NotifyPurchesersWhenNewOrderCreatedDomainEventHandler(
      IPurchaserRepository purchaserRepository,
      IMailerService mailerService)
    {
      this.purchaserRepository = purchaserRepository;
      this.mailerService = mailerService;
    }

    public async Task Handle(NewOrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
      string emailBody = string.Format(emailBodyTemplate, notification.Email);
      var purchasers = await purchaserRepository.GetAllWithEnabledNotification();
      purchasers?.ToList().ForEach(p => mailerService.Send(p.Email, emailSubject, emailBody));
    }
  }
}
