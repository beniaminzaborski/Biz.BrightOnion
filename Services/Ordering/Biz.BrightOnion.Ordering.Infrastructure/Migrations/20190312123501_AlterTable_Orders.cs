using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Biz.BrightOnion.Ordering.Infrastructure.Migrations
{
  [DbContext(typeof(OrderingContext))]
  [Migration("20190312123501_AlterTable_Orders")]
  public partial class _20190312123501_AlterTable_Orders : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn("PurchaserId", "Orders");
      migrationBuilder.DropColumn("Quantity", "Orders");

      migrationBuilder.CreateTable(
        name: "OrderItems",
        columns: table => new
        {
          Id = table.Column<long>(nullable: false)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
          OrderId = table.Column<long>(nullable: false),
          PurchaserId = table.Column<long>(nullable: false),
          Quantity = table.Column<int>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_OrderItems", x => x.Id);
          table.ForeignKey("FK_Orders_OrderItems", x => x.OrderId, "Orders", "Id");
        });
    }

    protected override void Down(MigrationBuilder migrationBuilder) { }
  }
}
