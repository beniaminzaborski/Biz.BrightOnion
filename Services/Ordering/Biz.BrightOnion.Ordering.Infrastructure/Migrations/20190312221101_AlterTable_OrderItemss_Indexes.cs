using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Biz.BrightOnion.Ordering.Infrastructure.Migrations
{
  [DbContext(typeof(OrderingContext))]
  [Migration("20190312221101_AlterTable_OrderItems_Indexes")]
  public partial class _20190312221101_AlterTable_OrderItems_Indexes : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddUniqueConstraint(
        "UQ__OrderItems_OrderId_PurchaserId", 
        "OrderItems",
        new string[] { "OrderId", "PurchaserId" });
    }

    protected override void Down(MigrationBuilder migrationBuilder) { }
  }
}
