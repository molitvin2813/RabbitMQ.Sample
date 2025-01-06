using EventBus.RabbitMQ.Interfaces;
using EventBus.RabbitMQ.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBus.RabbitMQ
{
    public class DefaultRabbitMQPersistentConnection
          : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        private readonly int _retryCount;
        IConnection _connection;
        bool _disposed;

        public DefaultRabbitMQPersistentConnection(
            ILogger<DefaultRabbitMQPersistentConnection> logger,
            IOptions<RabbitMQSettings> settings,
            int retryCount = 5)
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = settings.Value.HostName,
                UserName = settings.Value.UserName,
                Password = settings.Value.Password,
            };
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = retryCount;
        }

        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        public async Task<IChannel> CreateChannelAsync()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return await _connection.CreateChannelAsync();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        public async Task TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            var policy = RetryPolicy
                       .Handle<SocketException>()
                       .Or<BrokerUnreachableException>()
                       .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                       {
                           _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                       });

            await policy.Execute(async () =>
            {
                _connection = await _connectionFactory
                      .CreateConnectionAsync();
            });
        }
    }
}