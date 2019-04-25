using Biz.BrightOnion.Ordering.API.Application.Commands;
using FluentValidation;

namespace Biz.BrightOnion.Ordering.API.Application.Validations
{
  public class CreatePurchaserCommandValidator : AbstractValidator<CreatePurchaserCommand>
  {
    public CreatePurchaserCommandValidator()
    {
      RuleFor(command => command.PurchaserId).NotNull();
      RuleFor(command => command.Email).NotEmpty();
    }
  }
}
