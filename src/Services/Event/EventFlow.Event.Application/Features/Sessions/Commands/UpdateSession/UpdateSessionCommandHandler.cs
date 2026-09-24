using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Commands.UpdateSession
{
    public sealed class UpdateSessionCommandHandler(
        ISessionRepository sessionRepository,
        IVenueRepository venueRepository,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        IFileStorage fileStorage) : IRequestHandler<UpdateSessionCommand>
    {
        public async Task Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await sessionRepository.GetByIdAsync(request.Id, cancellationToken);
            if (session is null || session.EventId != request.EventId)
                throw new NotFoundException("Session not found.");

            var eventEntity = await eventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (eventEntity is null)
                throw new NotFoundException("Event not found.");

            if (request.VenueId.HasValue)
            {
                var venue = await venueRepository.GetByIdAsync(request.VenueId.Value, cancellationToken);
                if (venue is null || venue.EventId != request.EventId)
                    throw new NotFoundException("Venue not found for this event.");
            }

            var startUtc = ToUtc(request.StartTime, eventEntity.TimeZone);
            var endUtc = ToUtc(request.EndTime, eventEntity.TimeZone);

            if (startUtc.HasValue && startUtc.Value < eventEntity.StartDate)
                throw new ValidationException(new[] { new ValidationFailure("StartTime", "Session cannot start before the event starts.") });
            if (endUtc.HasValue && endUtc.Value > eventEntity.EndDate)
                throw new ValidationException(new[] { new ValidationFailure("EndTime", "Session cannot end after the event ends.") });

            if (request.VenueId.HasValue && startUtc.HasValue && endUtc.HasValue)
            {
                var existing = await sessionRepository.GetByEventIdAsync(request.EventId, cancellationToken);
                var conflict = existing.Any(x => x.Id != request.Id && x.VenueId == request.VenueId &&
                    x.StartTimeUtc.HasValue && x.EndTimeUtc.HasValue &&
                    startUtc < x.EndTimeUtc && endUtc > x.StartTimeUtc);
                if (conflict)
                    throw new ValidationException(new[] { new ValidationFailure("VenueId", "This venue is already assigned to another session during the selected time.") });
            }

            var imageUrl = session.ImageUrl;
            if (request.Image is not null)
            {
                var stored = await fileStorage.SaveAsync(request.Image, $"events/{request.EventId:D}/sessions", cancellationToken);
                imageUrl = stored.Url;
            }

            session.Update(request.Title, request.Description, request.SessionType, request.Capacity, startUtc, endUtc, request.VenueId, imageUrl);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static DateTime? ToUtc(DateTime? value, string timeZoneId)
        {
            if (!value.HasValue) return null;
            var timeZone = TimeZoneHelper.GetTimeZone(timeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified), timeZone);
        }
    }
}
