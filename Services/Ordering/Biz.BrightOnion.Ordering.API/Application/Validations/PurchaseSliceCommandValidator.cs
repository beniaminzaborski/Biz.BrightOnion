using Biz.BrightOnion.Ordering.API.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Validations
{
  public class PurchaseSliceCommandValidator : AbstractValidator<PurchaseSliceCommand>
  {
    public PurchaseSliceCommandValidator()
    {
      RuleFor(command => command.RoomId).NotNull();
      RuleFor(command => command.PurchaserId).NotNull();
      RuleFor(command => command.Quantity).NotNull();
    }
  }
}
