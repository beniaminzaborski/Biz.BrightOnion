using Biz.BrightOnion.Rooms.API.Entities;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Rooms.API.Migrations
{
  [Migration(20190225105208)]
  public class _201902251052_CreateTable_IntegrationEventLog : Migration
  {
    public override void Up()
    {
      Create.Table(nameof(IntegrationEventLog))
       .WithColumn(nameof(IntegrationEventLog.Id)).AsInt64().Identity().NotNullable().PrimaryKey()
       .WithColumn(nameof(IntegrationEventLog.EventId)).AsGuid().NotNullable().Unique()
       .WithColumn(nameof(IntegrationEventLog.EventCreationDate)).AsDateTime().NotNullable()
       .WithColumn(nameof(IntegrationEventLog.EventType)).AsAnsiString().NotNullable()
       .WithColumn(nameof(IntegrationEventLog.EventBody)).AsAnsiString().NotNullable() // TODO: Max lenght to unlimited
       .WithColumn(nameof(IntegrationEventLog.State)).AsAnsiString(14).NotNullable();
    }

    public override void Down() { }
  }
}
