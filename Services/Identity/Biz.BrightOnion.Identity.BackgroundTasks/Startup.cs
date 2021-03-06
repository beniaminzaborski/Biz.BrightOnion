﻿using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Biz.BrightOnion.EventBus;
using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.EventBus.RabbitMQ;
using Biz.BrightOnion.Identity.BackgroundTasks.Configuration;
using Biz.BrightOnion.Identity.BackgroundTasks.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Biz.BrightOnion.Identity.BackgroundTasks
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
      services.AddEventBus(Configuration);

      services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, PublishIntegrationEventsService>();

      var container = new ContainerBuilder();
      container.Populate(services);
      return new AutofacServiceProvider(container.Build());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
    }
  }

  public static class CustomExtensionMethods
  {
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
      var appSettingsSection = configuration.GetSection("AppSettings");
      var appSettings = appSettingsSection.Get<AppSettings>();

      // RabbitMQ Persistent Connection
      services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
      {
        var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

        var factory = new ConnectionFactory()
        {
          HostName = appSettings.EventBusConnection
        };

        if (!string.IsNullOrEmpty(appSettings.EventBusUserName))
        {
          factory.UserName = appSettings.EventBusUserName;
        }

        if (!string.IsNullOrEmpty(appSettings.EventBusPassword))
        {
          factory.Password = appSettings.EventBusPassword;
        }

        var retryCount = 5;
        if (appSettings.EventBusRetryCount.HasValue)
        {
          retryCount = appSettings.EventBusRetryCount.Value;
        }

        return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
      });

      var subscriptionClientName = appSettings.SubscriptionClientName;

      // RabbitMQ Event Bus
      services.AddSingleton<IEventBus, RabbitMqEventBus>(sp =>
      {
        var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
        var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
        var logger = sp.GetRequiredService<ILogger<RabbitMqEventBus>>();
        var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

        var retryCount = 5;
        if (appSettings.EventBusRetryCount.HasValue)
        {
          retryCount = appSettings.EventBusRetryCount.Value;
        }

        return new RabbitMqEventBus(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
      });

      services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

      return services;
    }
  }
}
