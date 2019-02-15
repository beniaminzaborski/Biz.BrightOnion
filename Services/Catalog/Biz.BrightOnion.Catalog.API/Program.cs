using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Biz.BrightOnion.Catalog.API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      // CreateWebHostBuilder(args).Build().Run();

      var webHost = CreateWebHostBuilder(args).Build();

      if (args.Length > 0 && args[0] == "dm")
      {
        using (var scope = webHost.Services.CreateScope())
        {
          UpdateDatabase(scope.ServiceProvider);
        }

        return;
      }

      webHost.Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();

    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
      // Instantiate the runner
      var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

      // Execute the migrations
      runner.MigrateUp();
    }
  }
}
