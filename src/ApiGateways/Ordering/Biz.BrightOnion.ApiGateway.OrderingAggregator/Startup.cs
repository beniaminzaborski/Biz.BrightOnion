using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Biz.BrightOnion.ApiGateway.OrderingAggregator.Config;
using Biz.BrightOnion.ApiGateway.OrderingAggregator.Infrastructure;
using Biz.BrightOnion.ApiGateway.OrderingAggregator.Services;
using Biz.BrightOnion.ServicesRegistry.Consul.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Biz.BrightOnion.ServicesRegistry.Consul;
using Consul;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Biz.BrightOnion.ServicesRegistry;
using Biz.BrightOnion.ServicesRegistry.Consul.LoadBalancers;
using OpenTracing;
using Jaeger.Reporters;
using Jaeger;
using Jaeger.Samplers;
using OpenTracing.Util;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator
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
            services.AddHealthChecks();

            services
              .AddOptions()
              .AddCustomConfiguration(Configuration)
              .AddCustomMvc(Configuration)
              // .AddCustomAuthentication(Configuration)
              .AddApplicationServices()
              .AddConsul(Configuration)
              .AddOpenTracing()
              .AddJaeger(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHealthChecks("/hc");

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();

            //app.UseSwagger()
            //    .UseSwaggerUI(c =>
            //    {
            //      c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Purchase BFF V1");
            //            //c.ConfigureOAuth2("Microsoft.eShopOnContainers.Web.Shopping.HttpAggregatorwaggerui", "", "", "Purchase BFF Swagger UI");
            //          });
        }
    }

    public static class ServiceCollectionExtensions
    {
        //public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        //{
        //  JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        //  var identityUrl = configuration.GetValue<string>("Urls:Identity");
        //  services.AddAuthentication(options =>
        //  {
        //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        //  }).AddJwtBearer(options =>
        //  {
        //    options.Authority = identityUrl;
        //    options.RequireHttpsMetadata = false;
        //    options.Audience = "brightonion";
        //    options.Events = new JwtBearerEvents()
        //    {
        //      OnAuthenticationFailed = async ctx =>
        //      {
        //        int i = 0;
        //      },
        //      OnTokenValidated = async ctx =>
        //      {
        //        int i = 0;
        //      }
        //    };
        //  });

        //  return services;
        //}

        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<ServicesRegistry.Consul.Config.Consul>(configuration.GetSection(nameof(ServicesRegistry.Consul.Config.Consul)))
                .Configure<Config.Jaeger>(configuration.GetSection(nameof(Config.Jaeger)));
        }
        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<UrlsConfig>(configuration.GetSection("Urls"));

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //services.AddSwaggerGen(options =>
            //{
            //  options.DescribeAllEnumsAsStrings();
            //  options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
            //  {
            //    Title = "Shopping Aggregator for Web Clients",
            //    Version = "v1",
            //    Description = "Shopping Aggregator for Web Clients",
            //    TermsOfService = "Terms Of Service"
            //  });

            //  options.AddSecurityDefinition("oauth2", new OAuth2Scheme
            //  {
            //    Type = "oauth2",
            //    Flow = "implicit",
            //    AuthorizationUrl = $"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize",
            //    TokenUrl = $"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/token",
            //    Scopes = new Dictionary<string, string>()
            //              {
            //                  { "webshoppingagg", "Shopping Aggregator for Web Clients" }
            //              }
            //  });

            //  options.OperationFilter<AuthorizeCheckOperationFilter>();
            //});

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
              builder => builder
              .SetIsOriginAllowed((host) => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //register delegating handlers
            services
              .AddTransient<HttpClientAuthorizationDelegatingHandler>()
              .AddTransient<HttpClientConsulServiceDiscoveryDelegatingHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //register http services
            services.AddHttpClient<IUserApiClient, UserApiClient>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddHttpMessageHandler<HttpClientConsulServiceDiscoveryDelegatingHandler>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddHttpClient<IRoomApiClient, RoomApiClient>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddHttpMessageHandler<HttpClientConsulServiceDiscoveryDelegatingHandler>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddHttpClient<IOrderApiClient, OrderApiClient>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddHttpMessageHandler<HttpClientConsulServiceDiscoveryDelegatingHandler>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
              .HandleTransientHttpError()
              .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
              .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        }

        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }

        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            var consulUrl = configuration.GetSection("Consul").Get<ServicesRegistry.Consul.Config.Consul>()?.Url;

            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = consulUrl;
                consulConfig.Address = new Uri(address);
            }));

            services.AddScoped<IServiceRegistry, ConsulServiceRegistry>();
            services.AddScoped<IServiceLoadBalancer, FakeGetRandomlyServiceLoadBalancer>();

            return services;
        }

        public static IServiceCollection AddJaeger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITracer>(serviceProvider =>
            {
                string serviceName = serviceProvider.GetRequiredService<IHostingEnvironment>().ApplicationName;

                var loggerFactory = new LoggerFactory();

                Environment.SetEnvironmentVariable("JAEGER_SERVICE_NAME", serviceName);

                var config = Jaeger.Configuration.FromEnv(loggerFactory);
                var tracer = config.GetTracer();

                if (!GlobalTracer.IsRegistered())
                    GlobalTracer.Register(tracer);

                return tracer;
            });

            return services;
        }
    }
}
