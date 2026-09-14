using EventFlow.Event.Application.Abstractions.Authentication;
using EventFlow.Event.Application.Abstractions.Authorization;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using MediatR;
using System.Security.Claims;

namespace EventFlow.Event.Application.Features.Events.Commands.CreateEvent
{
    //Handler for create events
    public sealed class CreateEventCommandHandler: IRequestHandler<CreateEventCommand, CreateEventResult>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public CreateEventCommandHandler(IEventRepository eventRepository,IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _eventRepository = eventRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;

        }

        public async Task<CreateEventResult> Handle(
            CreateEventCommand request,
            CancellationToken cancellationToken)
        {

            var startDate = DateTime.SpecifyKind(
                request.StartDate,
                DateTimeKind.Utc);

            var endDate = DateTime.SpecifyKind(
                request.EndDate,
                DateTimeKind.Utc);


            // Create the domain entity.
            var eventEntity = new EventEntity(
                request.Name,
                request.Description,
                request.EventType,
                request.SubType,
                startDate,
                endDate,
                request.TimeZone,
                _currentUserService.UserId);

            // Add the entity to the repository.
            await _eventRepository.AddAsync(eventEntity,cancellationToken);

            // Persist the changes.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateEventResult(eventEntity.Id);
        }
    }
}
