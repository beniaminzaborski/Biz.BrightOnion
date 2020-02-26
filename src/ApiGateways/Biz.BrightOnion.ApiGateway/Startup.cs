using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using OpenTracing;
using OpenTracing.Util;

namespace Biz.BrightOnion.ApiGateway
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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
              builder => builder
              .SetIsOriginAllowed((host) => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
            });

            services
                .AddOptions()
                .AddCustomConfiguration(Configuration)
                .AddOcelot(Configuration)
                .AddConsul()
                .AddPolly();

            services
                .AddOpenTracing()
                .AddJaeger(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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

            app.UseHttpsRedirection();
            app.UseWebSockets();
            app.UseMvc();
            app.UseOcelot();
        }
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<Config.Jaeger>(configuration.GetSection(nameof(Config.Jaeger)));
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
