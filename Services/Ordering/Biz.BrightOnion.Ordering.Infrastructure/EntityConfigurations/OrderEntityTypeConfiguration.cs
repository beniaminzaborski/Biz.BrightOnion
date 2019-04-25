using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Infrastructure.EntityConfigurations
{
  class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
  {
    public void Configure(EntityTypeBuilder<Order> orderConfiguration)
    {
      orderConfiguration.ToTable("Orders");

      orderConfiguration.HasKey(o => o.Id);

      // orderConfiguration.Ignore(b => b.DomainEvents);

      orderConfiguration.Property<long>("RoomId").IsRequired();
      orderConfiguration.Property<int>("SlicesPerPizza").IsRequired();
      orderConfiguration.Property<DateTime>("Day").IsRequired();
      orderConfiguration.Property<int>("TotalPizzas").IsRequired().HasDefaultValue(0);
      orderConfiguration.Property<int>("FreeSlicesToGrab").IsRequired().HasDefaultValue(0);

      var navigation = orderConfiguration.Metadata.FindNavigation(nameof(Order.OrderItems));

      orderConfiguration.Property<bool>("IsApproved");

      navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
  }
}
