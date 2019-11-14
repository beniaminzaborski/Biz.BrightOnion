using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Biz.BrightOnion.Ordering.Infrastructure.Migrations
{
  [DbContext(typeof(OrderingContext))]
  [Migration("20190312221901_AlterTable_Orders_Indexes")]
  public partial class _20190312221901_AlterTable_Orders_Indexes : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddUniqueConstraint(
        "UQ__Orders_RoomId_Day",
        "Orders",
        new string[] { "RoomId", "Day" });
    }

    protected override void Down(MigrationBuilder migrationBuilder) { }
  }
}
