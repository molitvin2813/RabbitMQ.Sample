using CatalogService.Application.Common.Response;
using CatalogService.Application.Interfaces;
using MediatR;
using SharedCollection.Interfaces;

namespace CatalogService.Application.Mediator.Commands.EventCommands.CreateEvent
{
    public class CreateEventHandler
        : IRequestHandler<CreateEventCommand, IServiceResponse>
    {
        public CreateEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private readonly IUnitOfWork _unitOfWork;

        public async Task<IServiceResponse> Handle(
            CreateEventCommand command,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.IntegrationEventOutboxService
                .SaveEventAsync(command.IntegrationEvent, true, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ServiceResponseWrite();
        }
    }
}
