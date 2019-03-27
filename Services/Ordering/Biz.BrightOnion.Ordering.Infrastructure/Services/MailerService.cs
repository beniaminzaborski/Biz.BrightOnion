using Biz.BrightOnion.Ordering.Domain.Services;
using Biz.BrightOnion.Ordering.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Biz.BrightOnion.Ordering.Infrastructure.Services
{
  public class MailerService : IMailerService
  {
    private readonly string server;
    private readonly int port;
    private readonly string sender;
    private readonly string user;
    private readonly string passwd;

    private readonly ILogger<MailerService> logger;

    public MailerService(
      IOptions<MailerOptions> mailerOptions,
      ILogger<MailerService> logger)
    {
      this.logger = logger;

      this.server = mailerOptions.Value.Server;
      this.port = mailerOptions.Value.Port;
      this.sender = mailerOptions.Value.Sender;
      this.user = mailerOptions.Value.User;
      this.passwd = mailerOptions.Value.Passwd;
    }

    public void Send(string to, string subject, string body)
    {
      try
      {
        SmtpClient client = new SmtpClient
        {
          Host = server,
          Port = port,
          EnableSsl = false,
          DeliveryMethod = SmtpDeliveryMethod.Network,
          Credentials = new System.Net.NetworkCredential(user, passwd),
          Timeout = 10000,
        };

        if (string.IsNullOrWhiteSpace(to))
          return;

        var recipents = to.Split(",");
        MailMessage message = new MailMessage
        {
          From = new MailAddress(sender)
        };
        foreach (var recipent in recipents)
        {
          try
          {
            message.To.Add(recipent);
          }
          catch { }
        }
        message.Body = body;
        message.Subject = subject;
        client.Send(message);
      }
      catch (Exception ex)
      {
        logger.LogError(ex.Message);
      }
    }
  }
}
