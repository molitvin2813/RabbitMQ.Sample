using EventBus.Abstraction.AbstractClass;
using EventBus.Abstraction.Interfaces;
using EventBus.RabbitMQ.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using SharedCollection.AbstractClass;
using SharedCollection.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BROKER_NAME = "event_bus";

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly int _retryCount;
        private readonly IServiceProvider _provider;
        private IChannel _consumerChannel;
        private string _queueName;


        public EventBusRabbitMQ(
            IRabbitMQPersistentConnection persistentConnection,
            ILogger<EventBusRabbitMQ> logger,
            IEventBusSubscriptionsManager subsManager,
            IServiceProvider provider,
            string queueName = null,
            int retryCount = 5)

        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager;
            _queueName = "order";
            _consumerChannel = CreateConsumerChannel().Result;
            _retryCount = retryCount;
            _provider = provider;
        }

        public async Task Publish(IntegrationEvent integrationEvent)
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy
                .Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", integrationEvent.IntegrationEventId, $"{time.TotalSeconds:n1}", ex.Message);
                });

            var eventName = integrationEvent.GetType().Name;

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", integrationEvent.IntegrationEventId, eventName);

            using (var channel = await _persistentConnection.CreateChannelAsync())
            {
                _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", integrationEvent.IntegrationEventId);

                await channel.ExchangeDeclareAsync(exchange: BROKER_NAME, type: "direct");

                var message = JsonConvert.SerializeObject(integrationEvent);
                var body = Encoding.UTF8.GetBytes(message);

                await policy.Execute(async () =>
                {
                    var properties = new BasicProperties();
                    properties.DeliveryMode = DeliveryModes.Persistent;

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", integrationEvent.IntegrationEventId);

                    await channel.BasicPublishAsync(
                        exchange: BROKER_NAME,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                });
            }
        }

        public async Task Subscribe<T, TH>()
            where T : class, IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            try
            {
                if (!_persistentConnection.IsConnected)
                {
                    await _persistentConnection.TryConnect();
                }

                using (var channel = await _persistentConnection.CreateChannelAsync())
                {
                    await channel.QueueBindAsync(queue: _queueName,
                        exchange: BROKER_NAME,
                        routingKey: typeof(T).Name);
                }

                _subsManager.Subscribe<T, TH>();

                StartBasicConsume();
            }
            catch (Exception ex)
            {

            }
        }

        public async Task Unsubscribe<T, TH>()
            where T : class, IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            try
            {
                _subsManager.Unsubscribe<T, TH>();

                if (!_persistentConnection.IsConnected)
                {
                    await _persistentConnection.TryConnect();
                }

                using (var channel = await _persistentConnection.CreateChannelAsync())
                {
                    await channel.QueueUnbindAsync(queue: _queueName,
                        exchange: BROKER_NAME,
                        routingKey: typeof(T).Name);

                    if (_subsManager.IsEmpty)
                    {
                        _queueName = string.Empty;
                        await _consumerChannel.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task<IChannel> CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = await _persistentConnection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: BROKER_NAME, type: "direct");

            await channel.QueueDeclareAsync(queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.CallbackExceptionAsync += async (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = await CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.ReceivedAsync += Consumer_Received;

                _consumerChannel.BasicConsumeAsync(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            await _consumerChannel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogTrace("Обработка события: {EventName}", eventName);

            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                var type = _subsManager.GetTypeEventByName(eventName);
                var integrationEvent = JsonConvert.DeserializeObject(message, type) as IntegrationEvent;
                var handlers = _subsManager.GetHandlersForEvent(integrationEvent);

                foreach (var subscription in handlers)
                {
                    var concreteType = typeof(Wrapper<>).MakeGenericType(integrationEvent.GetType());
                    var wrapper = Activator.CreateInstance(concreteType) ??
                        throw new InvalidOperationException($"Could not create wrapper type for {concreteType}"); ;
                    var wrapperBase = (WrapperBase)wrapper;

                    await wrapperBase.Handle(integrationEvent, _provider, default);
                }
            }
            else
            {
                _logger.LogWarning("Нет обработчика для события: {EventName}", eventName);
            }
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
            _subsManager.Clear();
        }
    }
}
