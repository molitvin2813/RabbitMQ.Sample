using MediatR;
using SharedCollection.AbstractClass;
using SharedCollection.Interfaces;

namespace CatalogService.Application.Mediator.Commands.EventCommands.CreateEvent
{
    public class CreateEventCommand
        : IRequest<IServiceResponse>
    {
        public IntegrationEvent IntegrationEvent { get; set; }
    }
}
