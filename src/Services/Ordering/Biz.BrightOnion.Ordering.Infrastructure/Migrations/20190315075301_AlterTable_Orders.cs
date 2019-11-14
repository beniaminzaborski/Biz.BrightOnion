using System;
using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Biz.BrightOnion.Ordering.Infrastructure.Migrations
{
  [DbContext(typeof(OrderingContext))]
  [Migration("20190315075301_AlterTable_Orders")]
  public partial class _20190315075301_AlterTable_Orders : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<int>(name: nameof(Order.TotalPizzas), table: "Orders", defaultValue: 0);
      migrationBuilder.AddColumn<int>(name: nameof(Order.FreeSlicesToGrab), table: "Orders", defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder) { }
  }
}
