using Biz.BrightOnion.Catalog.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Data
{
  public class ApplicationContext : DbContext
  {
    public ApplicationContext() : base() { }
    public ApplicationContext(DbContextOptions options) : base(options) { }

    public DbSet<Room> Rooms { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
    }
  }
}
