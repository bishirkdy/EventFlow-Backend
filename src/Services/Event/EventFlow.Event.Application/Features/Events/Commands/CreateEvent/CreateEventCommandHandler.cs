using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Commands.CreateEvent
{
    //Handler for create events
    public sealed class CreateEventCommandHandler: IRequestHandler<CreateEventCommand, CreateEventResult>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateEventCommandHandler(IEventRepository eventRepository,IUnitOfWork unitOfWork)
        {
            _eventRepository = eventRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateEventResult> Handle(
            CreateEventCommand request,
            CancellationToken cancellationToken)
        {
            // Create the domain entity.
            var eventEntity = new EventEntity(
                request.Name,
                request.Description,
                request.EventType,
                request.SubType,
                request.StartDate,
                request.EndDate,
                request.TimeZone,
                Guid.Empty);

            // Add the entity to the repository.
            await _eventRepository.AddAsync(eventEntity,cancellationToken);

            // Persist the changes.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateEventResult(eventEntity.Id);
        }
    }
}
