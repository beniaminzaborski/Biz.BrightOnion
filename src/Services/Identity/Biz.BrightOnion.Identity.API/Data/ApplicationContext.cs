using Biz.BrightOnion.Identity.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Data
{
  public class ApplicationContext : DbContext
  {
    public ApplicationContext() : base() { }
    public ApplicationContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
    }
  }
}
