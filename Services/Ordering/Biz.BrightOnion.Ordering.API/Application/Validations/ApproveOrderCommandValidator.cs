using Biz.BrightOnion.Ordering.API.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Validations
{
  public class ApproveOrderCommandValidator : AbstractValidator<ApproveOrderCommand>
  {
    public ApproveOrderCommandValidator()
    {
      RuleFor(command => command.OrderId).NotNull();
    }
  }
}
