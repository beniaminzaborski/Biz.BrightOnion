﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Biz.BrightOnion.Rooms.API.Configuration;
using Biz.BrightOnion.Rooms.API.Data;
using Biz.BrightOnion.Rooms.API.Infrastructure.Migrations;
using Biz.BrightOnion.Rooms.API.Repositories;
using Biz.BrightOnion.Rooms.API.Services;
using Biz.BrightOnion.EventBus;
using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.EventBus.RabbitMQ;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NHibernate;
using RabbitMQ.Client;

namespace Biz.BrightOnion.Rooms.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            services
                .AddDatabaseMigration(connectionString)
                .AddCustomHealthChecks(connectionString)
                .AddBusinessComponents(connectionString)
                .AddEventBus(Configuration)
                .AddCors()
                .AddCustomControllers()
                .AddJwtAuthentication(Configuration)
                .AddSwagger();
        }

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
                .UseCustomSwagger()
                .UseRouting()
                .UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                })
                .UseHealthChecks("/hc");
        }
    }

    public static class CatalogApiExtensions
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, string connectionString)
        {
            services
                .AddHealthChecks()
                .AddSqlServer(connectionString);

            return services;
        }

        public static IServiceCollection AddDatabaseMigration(this IServiceCollection services, string connectionString)
        {
            // Create database if does not exist
            DbMigrationHelper.EnsureCreated(connectionString);

            // SQL Server database migration
            services
              .AddFluentMigratorCore()
              .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(Migrations._20190206213801_CreateTable_Room).Assembly).For.Migrations()
            );

            return services;
        }

        public static IServiceCollection AddBusinessComponents(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<ISessionFactory>(NhSessionFactoryBuilder.Build(connectionString));
            services.AddScoped<ISession>(c => c.GetService<ISessionFactory>().OpenSession());

            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IIntegrationEventLogRepository, IntegrationEventLogRepository>();

            services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();

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

        public static IServiceCollection AddCustomControllers(this IServiceCollection services)
        {
            services
                .AddControllers()
                .AddNewtonsoftJson();

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
                .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{callingAssemblyName} {callingAssemblyVersion}"));
        }
    }
}