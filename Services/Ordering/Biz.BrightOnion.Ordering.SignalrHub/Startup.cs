﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
    public void ConfigureServices(IServiceCollection services)
    {
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

      services.AddSignalR();

      //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseCors("CorsPolicy");

      app.UseAuthentication();

      app.UseSignalR(routes =>
      {
        routes.MapHub<NotificationsHub>("/notificationhub", options =>
            options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransports.All);
      });

      //app.UseHttpsRedirection();
      //app.UseMvc();
    }
  }
}
