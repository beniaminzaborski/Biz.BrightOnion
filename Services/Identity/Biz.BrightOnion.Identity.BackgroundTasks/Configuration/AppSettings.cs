using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.BackgroundTasks.Configuration
{
  public class AppSettings
  {
    public virtual string Secret { get; set; }

    public virtual string EventBusConnection { get; set; }
    public virtual string EventBusUserName { get; set; }
    public virtual string EventBusPassword { get; set; }
    public virtual int? EventBusRetryCount { get; set; }
    public virtual string SubscriptionClientName { get; set; }
    public virtual int? PublishEventBatchSize { get; set; }
  }
}
