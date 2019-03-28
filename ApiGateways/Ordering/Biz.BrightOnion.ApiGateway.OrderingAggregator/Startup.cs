﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Biz.BrightOnion.ApiGateway.OrderingAggregator.Services;
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
      services
        .AddCustomMvc(Configuration)
        .AddCustomAuthentication(Configuration)
        .AddApplicationServices();

      // services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseCors("CorsPolicy");

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

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
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
      //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
      //var identityUrl = configuration.GetValue<string>("urls:identity");
      //services.AddAuthentication(options =>
      //{
      //  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      //  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

      //}).AddJwtBearer(options =>
      //{
      //  options.Authority = identityUrl;
      //  options.RequireHttpsMetadata = false;
      //  options.Audience = "webshoppingagg";
      //  options.Events = new JwtBearerEvents()
      //  {
      //    OnAuthenticationFailed = async ctx =>
      //    {
      //      int i = 0;
      //    },
      //    OnTokenValidated = async ctx =>
      //    {
      //      int i = 0;
      //    }
      //  };
      //});

      return services;
    }

    public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddOptions();
      // services.Configure<UrlsConfig>(configuration.GetSection("urls"));

      services.AddMvc()
          .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

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
      // services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      //register http services

      services.AddHttpClient<IUserApiClient, UserApiClient>()
          // .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
          .AddPolicyHandler(GetRetryPolicy())
          .AddPolicyHandler(GetCircuitBreakerPolicy());

      services.AddHttpClient<IRoomApiClient, RoomApiClient>()
          // .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
          .AddPolicyHandler(GetRetryPolicy())
          .AddPolicyHandler(GetCircuitBreakerPolicy());

      services.AddHttpClient<IOrderApiClient, OrderApiClient>()
          // .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
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
  }
}
