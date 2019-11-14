using Autofac;
using Biz.BrightOnion.Ordering.API.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Ordering.API.Infrastructure.AutofacModules
{
  public class ApplicationModule : Autofac.Module
  {
    public string QueriesConnectionString { get; }

    public ApplicationModule(string queriesConnectionString)
    {
      this.QueriesConnectionString = queriesConnectionString;
    }

    protected override void Load(ContainerBuilder builder)
    {
      builder.Register(c => new OrderQueries(QueriesConnectionString))
                .As<IOrderQueries>()
                .InstancePerLifetimeScope();
    }
  }
}
