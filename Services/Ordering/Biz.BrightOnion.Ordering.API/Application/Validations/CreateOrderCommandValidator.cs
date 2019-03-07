using Biz.BrightOnion.Ordering.API.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Validations
{
  public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
  {
    public CreateOrderCommandValidator()
    {
      RuleFor(command => command.RoomId).GreaterThan(0);
      RuleFor(command => command.PurchaserId).GreaterThan(0);
      RuleFor(command => command.Quantity).GreaterThan(0);
    }
  }
}
