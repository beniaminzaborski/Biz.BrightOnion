using System;
using Biz.BrightOnion.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Biz.BrightOnion.Ordering.Infrastructure.Migrations
{
  [DbContext(typeof(OrderingContext))]
  [Migration("20190322150101_CreateTable_Purchasers")]
  public partial class _20190322150101_CreateTable_Purchasers : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: "Purchasers",
        columns: table => new
        {
          Id = table.Column<long>(nullable: false),
          Email = table.Column<string>(nullable: false),
          NotificationEnabled = table.Column<bool>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Purchasers", x => x.Id);
        });
    }

    protected override void Down(MigrationBuilder migrationBuilder) { }
  }
}
