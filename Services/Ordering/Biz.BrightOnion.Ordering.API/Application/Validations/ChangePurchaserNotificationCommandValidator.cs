using Biz.BrightOnion.Ordering.API.Application.Commands;
using FluentValidation;

namespace Biz.BrightOnion.Ordering.API.Application.Validations
{
  public class ChangePurchaserNotificationCommandValidator : AbstractValidator<ChangePurchaserNotificationCommand>
  {
    public ChangePurchaserNotificationCommandValidator()
    {
      RuleFor(command => command.PurchaserId).NotNull();
    }
  }
}
