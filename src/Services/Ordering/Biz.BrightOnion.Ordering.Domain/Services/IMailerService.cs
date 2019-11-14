using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Domain.Services
{
  public interface IMailerService
  {
    void Send(string to, string subject, string body);
  }
}
