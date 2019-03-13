﻿using Autofac;
using Biz.BrightOnion.Ordering.API.Application.Behaviors;
using Biz.BrightOnion.Ordering.API.Application.Commands;
using Biz.BrightOnion.Ordering.API.Application.Validations;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Infrastructure.AutofacModules
{
  public class MediatorModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
          .AsImplementedInterfaces();

      // Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
      builder.RegisterAssemblyTypes(typeof(PurchaseSliceCommand).GetTypeInfo().Assembly)
          .AsClosedTypesOf(typeof(IRequestHandler<,>));

      // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
      //builder.RegisterAssemblyTypes(typeof(ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler).GetTypeInfo().Assembly)
      //    .AsClosedTypesOf(typeof(INotificationHandler<>));

      // Register the Command's Validators (Validators based on FluentValidation library)
      builder
          .RegisterAssemblyTypes(typeof(PurchaseSliceCommandValidator).GetTypeInfo().Assembly)
          .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
          .AsImplementedInterfaces();

      builder.Register<ServiceFactory>(context =>
      {
        var componentContext = context.Resolve<IComponentContext>();
        return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
      });

      //builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
      builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
      //builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
    }
  }
}
