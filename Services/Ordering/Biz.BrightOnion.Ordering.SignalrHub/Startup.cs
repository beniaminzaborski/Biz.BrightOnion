using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Biz.BrightOnion.EventBus;
using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.EventBus.RabbitMQ;
using Biz.BrightOnion.Ordering.SignalrHub.Configuration;
using Biz.BrightOnion.Ordering.SignalrHub.Events;
using Biz.BrightOnion.Ordering.SignalrHub.IntegrationEvents.EventHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Biz.BrightOnion.Ordering.SignalrHub
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
      services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

      services
        .AddCors(options =>
        {
          options.AddPolicy("CorsPolicy",
              builder => builder
              .AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed((host) => true)
              .AllowCredentials());
        });

      services.AddEventBus(Configuration);

      services.AddSignalR();

      //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      var container = new ContainerBuilder();
      container.Populate(services);
      return new AutofacServiceProvider(container.Build());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseCors("CorsPolicy");

      // app.UseAuthentication();

      app.UseSignalR(routes =>
      {
        routes.MapHub<NotificationsHub>("/notificationhub", options =>
            options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransports.All);
      });

      //app.UseHttpsRedirection();
      //app.UseMvc();

      ConfigureEventBus(app);
    }

    protected virtual void ConfigureEventBus(IApplicationBuilder app)
    {
      var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
      eventBus.Subscribe<OrderStatusChangedEvent, OrderStatusChangedEventHandler>();
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
      services.AddTransient<OrderStatusChangedEventHandler>();

      return services;
    }

    //public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    //{
    //  // configure strongly typed settings objects
    //  var appSettingsSection = configuration.GetSection("AppSettings");
    //  services.Configure<AppSettings>(appSettingsSection);

    //  // configure jwt authentication
    //  var appSettings = appSettingsSection.Get<AppSettings>();
    //  var key = Encoding.ASCII.GetBytes(appSettings.Secret);
    //  services.AddAuthentication(x =>
    //  {
    //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //  })
    //  .AddJwtBearer(x =>
    //  {
    //    x.RequireHttpsMetadata = false;
    //    x.SaveToken = true;
    //    x.TokenValidationParameters = new TokenValidationParameters
    //    {
    //      ValidateIssuerSigningKey = true,
    //      IssuerSigningKey = new SymmetricSecurityKey(key),
    //      ValidateIssuer = false,
    //      ValidateAudience = false
    //    };
    //  });

    //  return services;
    //}
  }
}
