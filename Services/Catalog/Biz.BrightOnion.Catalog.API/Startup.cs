using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biz.BrightOnion.Catalog.API.Data;
using Biz.BrightOnion.Catalog.API.Repositories;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHibernate;

namespace Biz.BrightOnion.Catalog.API
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
      string connectionString = Configuration.GetConnectionString("DefaultConnection");

      // Database migrations
      services
        .AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
          .AddSqlServer()
          .WithGlobalConnectionString(connectionString)
          .ScanIn(typeof(Migrations._20190206213801_CreateTable_Room).Assembly).For.Migrations()
      );

      services.AddSingleton<ISessionFactory>(NhSessionFactoryBuilder.Build(connectionString));
      services.AddScoped<ISession>(c => c.GetService<ISessionFactory>().OpenSession());

      services.AddScoped<IRoomRepository, RoomRepository>();

      services.AddCors();
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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

      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
}
