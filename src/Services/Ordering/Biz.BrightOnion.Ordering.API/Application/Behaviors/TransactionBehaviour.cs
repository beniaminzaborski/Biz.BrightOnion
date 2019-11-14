using Biz.BrightOnion.Ordering.API.Extensions;
using Biz.BrightOnion.Ordering.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Application.Behaviors
{
  public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  {
    private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> logger;
    private readonly OrderingContext dbContext;
    // private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public TransactionBehaviour(OrderingContext dbContext,
        /*IOrderingIntegrationEventService orderingIntegrationEventService,*/
        ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
    {
      this.dbContext = dbContext ?? throw new ArgumentException(nameof(OrderingContext));
      // _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentException(nameof(orderingIntegrationEventService));
      this.logger = logger ?? throw new ArgumentException(nameof(ILogger));
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
      var response = default(TResponse);
      var typeName = request.GetGenericTypeName();

      try
      {
        if (dbContext.HasActiveTransaction)
        {
          return await next();
        }

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
          using (var transaction = await dbContext.BeginTransactionAsync())
          // using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
          {
            logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

            response = await next();

            logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

            await dbContext.CommitTransactionAsync(transaction);
          }

          // await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync();
        });

        return response;
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

        throw;
      }
    }
  }
}
