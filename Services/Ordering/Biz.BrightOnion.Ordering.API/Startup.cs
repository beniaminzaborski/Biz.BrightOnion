using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Biz.BrightOnion.EventBus;
using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.EventBus.RabbitMQ;
using Biz.BrightOnion.Ordering.API.Configuration;
using Biz.BrightOnion.Ordering.API.EventHandlers;
using Biz.BrightOnion.Ordering.API.Events;
using Biz.BrightOnion.Ordering.API.Infrastructure.AutofacModules;
using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using Biz.BrightOnion.Ordering.Infrastructure;
using Biz.BrightOnion.Ordering.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;

namespace Biz.BrightOnion.Ordering.API
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
      services
        .AddCustomDbContext(Configuration)
        .AddEventBus(Configuration);

      services.AddScoped<IOrderRepository, OrderRepository>();

      services.AddCors();
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      services.AddJwtAuthentication(Configuration);

      var container = new ContainerBuilder();
      container.Populate(services);
      container.RegisterModule(new MediatorModule());
      container.RegisterModule(new ApplicationModule(Configuration.GetConnectionString("DefaultConnection")));
      return new AutofacServiceProvider(container.Build());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      // global cors policy
      app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

      app.UseAuthentication();

      app.UseHttpsRedirection();
      app.UseMvc();

      ConfigureEventBus(app);
    }

    protected virtual void ConfigureEventBus(IApplicationBuilder app)
    {
      var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
      eventBus.Subscribe<UserNotificationChangedEvent, UserNotificationChangedEventHandler>();
      eventBus.Subscribe<RoomNameChangedEvent, RoomNameChangedEventHandler>();
      eventBus.Subscribe<RoomDeletedEvent, RoomDeletedEventHandler>();
    }
  }

  public static class CustomExtensionMethods
  {
    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddEntityFrameworkSqlServer()
        .AddDbContext<OrderingContext>(options =>
        {
          options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptionsAction: sqlOptions =>
            {
              sqlOptions.MigrationsAssembly(typeof(OrderingContext).GetTypeInfo().Assembly.GetName().Name);
              sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            });
        },
            ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
        );

      return services;
    }

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
      services.AddTransient<UserNotificationChangedEventHandler>();
      services.AddTransient<RoomNameChangedEventHandler>();
      services.AddTransient<RoomDeletedEventHandler>();

      return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
      // configure strongly typed settings objects
      var appSettingsSection = configuration.GetSection("AppSettings");
      services.Configure<AppSettings>(appSettingsSection);

      // configure jwt authentication
      var appSettings = appSettingsSection.Get<AppSettings>();
      var key = Encoding.ASCII.GetBytes(appSettings.Secret);
      services.AddAuthentication(x =>
      {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(x =>
      {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false
        };
      });

      return services;
    }
  }
}
