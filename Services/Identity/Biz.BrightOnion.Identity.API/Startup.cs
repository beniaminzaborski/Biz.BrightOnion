using System;
using System.IO;
using System.Reflection;
using System.Text;
using Autofac;
using Biz.BrightOnion.EventBus;
using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.EventBus.RabbitMQ;
using Biz.BrightOnion.Identity.API.Configuration;
using Biz.BrightOnion.Identity.API.Data;
using Biz.BrightOnion.Identity.API.Repositories;
using Biz.BrightOnion.Identity.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace Biz.BrightOnion.Identity.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCustomHealthChecks()
                .AddEntityFramework(Configuration)
                .AddBusinessComponents()
                .AddEventBus(Configuration)
                .AddCors()
                .AddCustomControllers()
                .AddJwtAuthentication(Configuration)
                .AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app
                .UseHttpsRedirection()
                .UseRouting()
                .UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                })
                .UseHealthChecks("/hc")
                .UseCustomSwagger();
        }
    }

    public static class IdentityApiExtensions
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationContext>();
            return services;
        }

        public static IServiceCollection AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }

        public static IServiceCollection AddCustomControllers(this IServiceCollection services)
        {
            services
                .AddControllers()
                .AddNewtonsoftJson();

            return services;
        }

        public static IServiceCollection AddBusinessComponents(this IServiceCollection services)
        {
            services
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IIntegrationEventLogRepository, IntegrationEventLogRepository>()
                .AddScoped<IPasswordHasher, Md5PasswordHasher>()
                .AddScoped<IAuthenticationService, JwtAuthenticationService>()
                .AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();
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

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var callingAssemblyName = callingAssembly.GetName().Name;
            var callingAssemblyMajorVersion = $"v{callingAssembly.GetName().Version?.Major}";
            var callingAssemblyVersion = $"v{callingAssembly.GetName().Version?.ToString()}";

            return services.AddSwaggerGen(c =>
            {
                var securityRequirements = new OpenApiSecurityRequirement();
                securityRequirements.Add(new OpenApiSecurityScheme() { Scheme = "Bearer" }, new string[] { });
                c.SwaggerDoc(callingAssemblyMajorVersion, new OpenApiInfo { Title = callingAssemblyName, Version = callingAssemblyVersion });
                c.AddSecurityRequirement(securityRequirements);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() { In = ParameterLocation.Header, Description = "Please insert token with Bearer into field", Name = "Authorization", Type = SecuritySchemeType.ApiKey });
                //// Set the comments path for the Swagger JSON and UI.
                //var basePath = AppContext.BaseDirectory;
                //var xmlPath = Path.Combine(basePath, $"{callingAssemblyName}.xml");
                //c.IncludeXmlComments(xmlPath);
            });
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var callingAssemblyName = callingAssembly.GetName().Name;
            var callingAssemblyMajorVersion = $"v{callingAssembly.GetName().Version?.Major}";
            var callingAssemblyVersion = $"v{callingAssembly.GetName().Version?.ToString()}";

            return app
                .UseSwagger()
                .UseSwaggerUI(c => c.SwaggerEndpoint("./v1/swagger.json", $"{callingAssemblyName} {callingAssemblyVersion}"));
        }
    }
}
