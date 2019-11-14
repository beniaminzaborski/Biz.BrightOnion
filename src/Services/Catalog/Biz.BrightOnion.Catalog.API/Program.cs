using System;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Biz.BrightOnion.Catalog.API
{
    public class Program
  {
    public static void Main(string[] args)
    {
      var host = CreateHostBuilder(args).Build();

      if (args.Length > 0 && args[0] == "dm")
      {
        using (var scope = host.Services.CreateScope())
        {
          UpdateDatabase(scope.ServiceProvider);
        }

        return;
      }

      host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(serverOptions => {})
                .UseStartup<Startup>();
            });

    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
      // Instantiate the runner
      var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

      // Execute the migrations
      runner.MigrateUp();
    }
  }
}
