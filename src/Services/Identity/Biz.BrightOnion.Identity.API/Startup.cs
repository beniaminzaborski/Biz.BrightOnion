using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Autofac;
using Biz.BrightOnion.EventBus;
using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.EventBus.RabbitMQ;
using Biz.BrightOnion.Identity.API.Configuration;
using Biz.BrightOnion.Identity.API.Data;
using Biz.BrightOnion.Identity.API.HealthChecks;
using Biz.BrightOnion.Identity.API.Repositories;
using Biz.BrightOnion.Identity.API.Services;
using Consul;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTracing;
using OpenTracing.Util;
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
                .AddCors()
                .AddCustomControllers()
                .AddJwtAuthentication(Configuration)
                .AddConsul(Configuration)
                .AddSwagger();

            services
                .AddOpenTracing()
                .AddJaeger(Configuration);

            services
                .AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("database_health_check");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime, IWebHostEnvironment env, ApplicationContext context)
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

            // Create database if does not exist
            context.Database.EnsureCreated();

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
                    endpoints.MapHealthChecks("/health");
                })
                //.UseHealthChecks("/health")
                .UseConsul(appLifetime, Configuration);

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapHealthChecks("/health");
            //});
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
            });
        }

        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            var consulUrl = configuration.GetValue<string>("AppSettings:ConsulConnection");

            return services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = consulUrl;
                consulConfig.Address = new Uri(address);
            }));
        }

        public static IServiceCollection AddJaeger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITracer>(serviceProvider =>
            {
                string serviceName = serviceProvider.GetRequiredService<IWebHostEnvironment>().ApplicationName;

                var loggerFactory = new LoggerFactory();

                var jaegerAgentHost = configuration.GetValue<string>("AppSettings:JaegerAgentHost");
                var jaegerAgentPort = configuration.GetValue<int>("AppSettings:JaegerAgentPort");

                var senderConfig = new Jaeger.Configuration.SenderConfiguration(loggerFactory)
                    .WithAgentHost(jaegerAgentHost)
                    /*.WithAgentPort(jaegerConfig.AgentPort)*/;

                var reporter = new RemoteReporter.Builder()
                    .WithLoggerFactory(loggerFactory)
                    .WithSender(senderConfig.GetSender())
                    .Build();

                var tracer = new Tracer.Builder(serviceName)
                    .WithLoggerFactory(loggerFactory)
                    .WithReporter(reporter)
                    .WithSampler(new ConstSampler(true))
                    .Build();

                if (!GlobalTracer.IsRegistered())
                    GlobalTracer.Register(tracer);

                return tracer;
            });

            return services;
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

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IHostApplicationLifetime appLifetime, IConfiguration configuration)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

            // Get service name
            var serviceName = configuration.GetValue<string>("AppSettings:ServiceName");

            // Get server IP address
            //var features = app.Properties["server.Features"] as FeatureCollection;
            //var addresses = features.Get<IServerAddressesFeature>();
            //var address = addresses.Addresses.First();

            var name = Dns.GetHostName(); // get container id
            var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

            // Register service with consul
            //var uri = new Uri(address);
            var agentReg = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString(),
                Name = serviceName,
                Address = ip.ToString(),
                Port = 80
                //Checks = new AgentCheckRegistration[] {new AgentCheckRegistration
                //{
                //    HTTP = $"http://{serviceName}/hc",
                //    Timeout = TimeSpan.FromSeconds(3),
                //    Interval = TimeSpan.FromSeconds(10)
                //}
            };

            consulClient.Agent.ServiceRegister(agentReg).GetAwaiter().GetResult();

            appLifetime.ApplicationStopping.Register(() => {
                consulClient.Agent.ServiceDeregister(agentReg.ID).Wait();
            });

            return app;
        }
    }
}
