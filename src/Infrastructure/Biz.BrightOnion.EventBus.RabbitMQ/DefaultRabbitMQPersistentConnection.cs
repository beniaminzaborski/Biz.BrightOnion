using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Biz.BrightOnion.EventBus.RabbitMQ
{
  public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
  {
    private readonly IConnectionFactory connectionFactory;
    private readonly ILogger<DefaultRabbitMQPersistentConnection> logger;
    private readonly int retryCount;
    IConnection connection;
    bool disposed;

    object sync_root = new object();

    public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMQPersistentConnection> logger, int retryCount = 5)
    {
      this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
      this.retryCount = retryCount;
    }

    public bool IsConnected
    {
      get
      {
        return connection != null && connection.IsOpen && !disposed;
      }
    }

    public IModel CreateModel()
    {
      if (!IsConnected)
      {
        throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
      }

      return connection.CreateModel();
    }

    public void Dispose()
    {
      if (disposed) return;

      disposed = true;

      try
      {
        connection.Dispose();
      }
      catch (IOException ex)
      {
        logger.LogCritical(ex.ToString());
      }
    }

    public bool TryConnect()
    {
      logger.LogInformation("RabbitMQ Client is trying to connect");

      lock (sync_root)
      {
        var policy = RetryPolicy.Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
              logger.LogWarning(ex.ToString());
            }
        );

        policy.Execute(() =>
        {
          connection = connectionFactory
                .CreateConnection();
        });

        if (IsConnected)
        {
          connection.ConnectionShutdown += OnConnectionShutdown;
          connection.CallbackException += OnCallbackException;
          connection.ConnectionBlocked += OnConnectionBlocked;

          logger.LogInformation($"RabbitMQ persistent connection acquired a connection {connection.Endpoint.HostName} and is subscribed to failure events");

          return true;
        }
        else
        {
          logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

          return false;
        }
      }
    }

    private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
    {
      if (disposed) return;

      logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

      TryConnect();
    }

    void OnCallbackException(object sender, CallbackExceptionEventArgs e)
    {
      if (disposed) return;

      logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

      TryConnect();
    }

    void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
    {
      if (disposed) return;

      logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

      TryConnect();
    }
  }
}
