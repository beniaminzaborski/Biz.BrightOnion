using Autofac;
using Biz.BrightOnion.EventBus.Abstractions;
using Biz.BrightOnion.EventBus.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Biz.BrightOnion.EventBus.RabbitMQ
{
  public class RabbitMqEventBus : IEventBus, IDisposable
  {
    const string BROKER_NAME = "brightonion_event_bus";

    private readonly IRabbitMQPersistentConnection persistentConnection;
    private readonly ILogger<RabbitMqEventBus> logger;
    private readonly IEventBusSubscriptionsManager subsManager;
    private readonly ILifetimeScope autofac;
    private readonly string AUTOFAC_SCOPE_NAME = "brightonion_event_bus";
    private readonly int retryCount;

    private IModel consumerChannel;
    private string queueName;

    public RabbitMqEventBus(IRabbitMQPersistentConnection persistentConnection, ILogger<RabbitMqEventBus> logger,
        ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, string queueName = null, int retryCount = 5)
    {
      this.persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
      this.subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
      this.queueName = queueName;
      consumerChannel = CreateConsumerChannel();
      this.autofac = autofac;
      this.retryCount = retryCount;
      this.subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
    }

    private void SubsManager_OnEventRemoved(object sender, string eventName)
    {
      if (!persistentConnection.IsConnected)
      {
        persistentConnection.TryConnect();
      }

      using (var channel = persistentConnection.CreateModel())
      {
        channel.QueueUnbind(queue: queueName,
            exchange: BROKER_NAME,
            routingKey: eventName);

        if (subsManager.IsEmpty)
        {
          queueName = string.Empty;
          consumerChannel.Close();
        }
      }
    }

    public void Publish(IntegrationEvent @event)
    {
      if (!persistentConnection.IsConnected)
      {
        persistentConnection.TryConnect();
      }

      var policy = RetryPolicy.Handle<BrokerUnreachableException>()
          .Or<SocketException>()
          .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
          {
            logger.LogWarning(ex.ToString());
          });

      using (var channel = persistentConnection.CreateModel())
      {
        var eventName = @event.GetType()
            .Name;

        channel.ExchangeDeclare(exchange: BROKER_NAME,
                            type: "direct");

        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        policy.Execute(() =>
        {
          var properties = channel.CreateBasicProperties();
          properties.DeliveryMode = 2; // persistent

          channel.BasicPublish(exchange: BROKER_NAME,
                           routingKey: eventName,
                           mandatory: true,
                           basicProperties: properties,
                           body: body);
        });
      }
    }

    public void SubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
      DoInternalSubscription(eventName);
      subsManager.AddDynamicSubscription<TH>(eventName);
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
      var eventName = subsManager.GetEventKey<T>();
      DoInternalSubscription(eventName);
      subsManager.AddSubscription<T, TH>();
    }

    private void DoInternalSubscription(string eventName)
    {
      var containsKey = subsManager.HasSubscriptionsForEvent(eventName);
      if (!containsKey)
      {
        if (!persistentConnection.IsConnected)
        {
          persistentConnection.TryConnect();
        }

        using (var channel = persistentConnection.CreateModel())
        {
          channel.QueueBind(queue: queueName,
                            exchange: BROKER_NAME,
                            routingKey: eventName);
        }
      }
    }

    public void Unsubscribe<T, TH>()
        where TH : IIntegrationEventHandler<T>
        where T : IntegrationEvent
    {
      subsManager.RemoveSubscription<T, TH>();
    }

    public void UnsubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
      subsManager.RemoveDynamicSubscription<TH>(eventName);
    }

    public void Dispose()
    {
      if (consumerChannel != null)
      {
        consumerChannel.Dispose();
      }

      subsManager.Clear();
    }

    private IModel CreateConsumerChannel()
    {
      if (!persistentConnection.IsConnected)
      {
        persistentConnection.TryConnect();
      }

      var channel = persistentConnection.CreateModel();

      channel.ExchangeDeclare(exchange: BROKER_NAME,
                           type: "direct");

      channel.QueueDeclare(queue: queueName,
                           durable: true,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);


      var consumer = new EventingBasicConsumer(channel);
      consumer.Received += async (model, ea) =>
      {
        var eventName = ea.RoutingKey;
        var message = Encoding.UTF8.GetString(ea.Body);

        await ProcessEvent(eventName, message);

        channel.BasicAck(ea.DeliveryTag, multiple: false);
      };

      channel.BasicConsume(queue: queueName,
                           autoAck: false,
                           consumer: consumer);

      channel.CallbackException += (sender, ea) =>
      {
        consumerChannel.Dispose();
        consumerChannel = CreateConsumerChannel();
      };

      return channel;
    }

    private async Task ProcessEvent(string eventName, string message)
    {
      if (subsManager.HasSubscriptionsForEvent(eventName))
      {
        using (var scope = autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
        {
          var subscriptions = subsManager.GetHandlersForEvent(eventName);
          foreach (var subscription in subscriptions)
          {
            if (subscription.IsDynamic)
            {
              var handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
              if (handler == null) continue;
              dynamic eventData = JObject.Parse(message);
              await handler.Handle(eventData);
            }
            else
            {
              var handler = scope.ResolveOptional(subscription.HandlerType);
              if (handler == null) continue;
              var eventType = subsManager.GetEventTypeByName(eventName);
              var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
              var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
              await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
            }
          }
        }
      }
    }
  }
}
