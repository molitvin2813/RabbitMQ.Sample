using EventBus.Abstraction.Interfaces;
using EventBus.RabbitMQ.Interfaces;
using SharedCollection.AbstractClass;
using SharedCollection.Interfaces;
using System.Collections.Concurrent;

namespace EventBus.RabbitMQ
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new ConcurrentDictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
        }

        private readonly ConcurrentDictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;

        public bool IsEmpty => _handlers.Count == 0;
        public void Clear() => _handlers.Clear();

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            return HasSubscriptionsForEvent(typeof(T).Name);
        }

        public bool HasSubscriptionsForEvent(string eventName)
        {
            return _handlers.ContainsKey(eventName);
        }

        public Type GetTypeEventByName(string eventName)
        {
            return _eventTypes.First(x => x.Name == eventName);
        }

        public IEnumerable<Type> GetHandlersForEvent<T>(T integrationEvent)
           where T : IntegrationEvent
        {
            return _handlers[integrationEvent.GetType().Name];
        }

        public void Subscribe<T, TH>()
            where T : class, IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _handlers.TryGetValue(typeof(T).Name, out var handlers);
            if (handlers != null)
            {
                handlers.Add(typeof(TH));
            }
            else
            {
                _handlers.TryAdd(typeof(T).Name, new List<Type>() { typeof(TH) });
                _eventTypes.Add(typeof(T));
            }
        }

        public void Unsubscribe<T, TH>()
            where T : class, IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _handlers.TryGetValue(typeof(T).Name, out var handlers);
            handlers?.Remove(typeof(TH));
        }
    }
}