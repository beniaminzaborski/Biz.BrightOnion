using Biz.BrightOnion.Identity.API.Data;
using Biz.BrightOnion.Identity.API.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Biz.BrightOnion.Identity.API.Migrations
{
  [DbContext(typeof(ApplicationContext))]
  [Migration("20190225145701_CreateTable_IntegrationEventLogs")]
  public partial class _20190225145701_CreateTable_IntegrationEventLogs : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: "IntegrationEventLogs",
        columns: table => new
        {
          Id = table.Column<long>(nullable: false)
            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
          EventId = table.Column<Guid>(nullable: false),
          EventCreationDate = table.Column<DateTime>(nullable: false),
          EventType = table.Column<string>(nullable: false, maxLength: 255),
          EventBody = table.Column<string>(nullable: false),
          State = table.Column<string>(nullable: false, maxLength: 14)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_IntegrationEventLogs", x => x.Id);
          table.UniqueConstraint("IX_IntegrationEventLogs_EventId", x => x.EventId);
        });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(name: "IntegrationEventLogs");
    }
  }
}
