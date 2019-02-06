using Biz.BrightOnion.Catalog.API.Entities;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Migrations
{
  [Migration(20190206213801)]
  public class _20190206213801_CreateTable_Room : Migration
  {
    public override void Up()
    {
      Create.Table(nameof(Room))
        .WithColumn(nameof(Room.Id)).AsInt64().Identity().NotNullable().PrimaryKey()
        .WithColumn(nameof(Room.Name)).AsAnsiString(25).NotNullable();
    }

    public override void Down() { }
  }
}
