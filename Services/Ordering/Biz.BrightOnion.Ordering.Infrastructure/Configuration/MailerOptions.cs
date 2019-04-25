using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Infrastructure.Configuration
{
  public class MailerOptions
  {
    public virtual string Server { get; set; }
    public virtual int Port { get; set; }
    public virtual string Sender { get; set; }
    public virtual string User { get; set; }
    public virtual string Passwd { get; set; }
  }
}
