﻿using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
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
      orderConfiguration.Property<DateTime>("Day").IsRequired();

      var navigation = orderConfiguration.Metadata.FindNavigation(nameof(Order.OrderItems));

      navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
  }
}