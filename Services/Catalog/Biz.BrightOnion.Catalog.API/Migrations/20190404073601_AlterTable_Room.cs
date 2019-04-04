using Biz.BrightOnion.Catalog.API.Entities;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Migrations
{
  [Migration(20190404073601)]
  public class _20190404073601_AlterTable_Room : Migration
  {
    public override void Up()
    {
      Alter.Table(nameof(Room)).AddColumn(nameof(Room.ManagerId)).AsInt64().Nullable();
    }

    public override void Down() { }
  }
}
