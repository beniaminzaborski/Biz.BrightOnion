using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using Biz.BrightOnion.Ordering.Domain.AggregatesModel.PurchaserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Infrastructure.EntityConfigurations
{
  class PurchaserEntityTypeConfiguration : IEntityTypeConfiguration<Purchaser>
  {
    public void Configure(EntityTypeBuilder<Purchaser> purchaserConfiguration)
    {
      purchaserConfiguration.ToTable("Purchasers");

      purchaserConfiguration.HasKey(o => o.Id);

      // orderConfiguration.Ignore(b => b.DomainEvents);

      purchaserConfiguration.Property<string>("Email").IsRequired();
      purchaserConfiguration.Property<bool>("NotificationEnabled")/*.IsRequired().HasDefaultValue(false)*/;
    }
  }
}
