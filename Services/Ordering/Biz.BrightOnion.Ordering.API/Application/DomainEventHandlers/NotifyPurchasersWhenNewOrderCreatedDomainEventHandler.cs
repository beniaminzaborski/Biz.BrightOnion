using Biz.BrightOnion.Ordering.Domain.AggregatesModel.PurchaserAggregate;
using Biz.BrightOnion.Ordering.Domain.Events;
using Biz.BrightOnion.Ordering.Domain.Services;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.DomainEventHandlers
{
  public class NotifyPurchasersWhenNewOrderCreatedDomainEventHandler : INotificationHandler<NewOrderCreatedDomainEvent>
  {
    private const string emailSubject = "Bright Onion";
    private const string emailBodyTemplate = "Oops someone is hungry. The pizza has been just opened in {0} room by {1} on {2}. Can you join me? Let's get some pizza.";

    private readonly IPurchaserRepository purchaserRepository;
    private readonly IMailerService mailerService;

    public NotifyPurchasersWhenNewOrderCreatedDomainEventHandler(
      IPurchaserRepository purchaserRepository,
      IMailerService mailerService)
    {
      this.purchaserRepository = purchaserRepository;
      this.mailerService = mailerService;
    }

    public async Task Handle(NewOrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
      var purchaser = await purchaserRepository.Get(notification.PurchaserId);
      string emailBody = string.Format(emailBodyTemplate, notification.RoomName, purchaser?.Email, notification.Day.ToString("yyy-MM-dd"));
      var purchasers = await purchaserRepository.GetAllWithEnabledNotification();
      purchasers?.ToList().ForEach(p => mailerService.Send(p.Email, emailSubject, emailBody));
    }
  }
}
