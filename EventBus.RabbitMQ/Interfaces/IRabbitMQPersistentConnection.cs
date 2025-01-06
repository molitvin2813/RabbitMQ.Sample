using RabbitMQ.Client;

namespace EventBus.RabbitMQ.Interfaces
{
    public interface IRabbitMQPersistentConnection
         : IDisposable
    {
        bool IsConnected { get; }

        Task TryConnect();

        Task<IChannel> CreateChannelAsync();
    }
}
