using EventBus.Abstraction.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using SharedCollection.Interfaces;

namespace EventBus.Abstraction.AbstractClass
{
    public abstract class WrapperBase
    {
        public abstract Task Handle(
            object integrationEvent,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }

    public abstract class EventHandlerWrapper : WrapperBase
    {
        public abstract Task Handle(
            IIntegrationEvent integrationEvent,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }

    public class Wrapper<TEvent> : EventHandlerWrapper
    where TEvent : class, IIntegrationEvent
    {
        public override async Task Handle(
            object request,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            await Handle((IIntegrationEvent)request, serviceProvider, cancellationToken).ConfigureAwait(false);
        }

        public override async Task Handle(
            IIntegrationEvent request,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var dd = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<TEvent>>();
            await dd.Handle((TEvent)request, cancellationToken);
        }
    }
}
