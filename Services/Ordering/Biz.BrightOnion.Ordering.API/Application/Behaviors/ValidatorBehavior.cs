using Biz.BrightOnion.Ordering.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biz.BrightOnion.Ordering.API.Extensions;

namespace Biz.BrightOnion.Ordering.API.Application.Behaviors
{
  public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  {
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> logger;
    private readonly IValidator<TRequest>[] validators;

    public ValidatorBehavior(IValidator<TRequest>[] validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
    {
      this.validators = validators;
      this.logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
      var typeName = request.GetGenericTypeName();

      logger.LogInformation("----- Validating command {CommandType}", typeName);

      var failures = validators
          .Select(v => v.Validate(request))
          .SelectMany(result => result.Errors)
          .Where(error => error != null)
          .ToList();

      if (failures.Any())
      {
        logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);

        throw new OrderingDomainException(
            $"Command Validation Errors for type {typeof(TRequest).Name}", new ValidationException("Validation exception", failures));
      }

      return await next();
    }
  }
}
