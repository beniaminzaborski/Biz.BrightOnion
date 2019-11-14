using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Infrastructure.EntityConfigurations
{
  class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
  {
    public void Configure(EntityTypeBuilder<OrderItem> orderItemConfiguration)
    {
      orderItemConfiguration.ToTable("OrderItems");

      orderItemConfiguration.HasKey(o => o.Id);

      // orderItemConfiguration.Ignore(b => b.DomainEvents);

      orderItemConfiguration.Property<long>("OrderId")
          .IsRequired();

      orderItemConfiguration.Property<long>("PurchaserId")
          .IsRequired();

      orderItemConfiguration.Property<int>("Quantity")
          .IsRequired();
    }
  }
}
