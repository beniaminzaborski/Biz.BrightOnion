using System;
using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Biz.BrightOnion.Ordering.Infrastructure.Migrations
{
  [DbContext(typeof(OrderingContext))]
  [Migration("20190329130901_AlterTable_Orders")]
  public partial class _20190329130901_AlterTable_Orders : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<bool>(name: nameof(Order.IsApproved), table: "Orders", defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder) { }
  }
}
