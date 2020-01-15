using Biz.BrightOnion.Rooms.API.Infrastructure.Entities;
using Biz.BrightOnion.Rooms.API.Infrastructure.NhMappingConventions;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.SqlCommand;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Rooms.API.Data
{
  public class NhSessionFactoryBuilder
  {
    public static ISessionFactory Build(string connectionString)
    {
      return Fluently.Configure()
        .Database(MsSqlConfiguration.MsSql2012
          .ConnectionString(connectionString)
          .UseOuterJoin()
#if DEBUG
          .ShowSql()
#endif
        )
        .Mappings(m =>
          m.AutoMappings.Add(
            AutoMap.Assembly(typeof(Entity).Assembly, new NHAutomappingConfiguration())
                // .AddEntityAssembly(typeof(Entity).Assembly)
                .UseOverridesFromAssembly(typeof(Entity).Assembly)
                .Conventions.Setup(c =>
                {
                  c.Add<ForeignKeyMappingConvention>();
                  c.Add(ConventionBuilder.Id.Always(x => x.GeneratedBy.Identity()));
                })
            )
         )
#if DEBUG
         .ExposeConfiguration(c => c.SetInterceptor(new SqlTraceInterceptor()))
#endif
      .BuildSessionFactory();
    }
  }

  public class NHAutomappingConfiguration : DefaultAutomappingConfiguration
  {
    public override bool ShouldMap(Type type)
    {
      bool shouldMap = typeof(Entity).IsAssignableFrom(type);
      return shouldMap;
    }

    public override bool ShouldMap(FluentNHibernate.Member member)
    {
      bool shouldMap = base.ShouldMap(member) && member.CanWrite;
      return shouldMap;
    }

    //public override bool IsComponent(Type type)
    //{
    //  return Attribute.GetCustomAttribute(type, typeof(ValueObjectAttribute)) != null;
    //}
  }

  public class SqlTraceInterceptor : EmptyInterceptor
  {
    public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
    {
      Trace.WriteLine(sql.ToString());
      return sql;
    }
  }
}
