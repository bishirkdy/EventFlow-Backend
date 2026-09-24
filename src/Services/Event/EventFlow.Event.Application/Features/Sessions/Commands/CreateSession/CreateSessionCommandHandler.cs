using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Exceptions;
using EventFlow.Event.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Commands.CreateSession
{
    public sealed class CreateSessionCommandHandler(
        ISessionRepository sessionRepository,
        ISectionRepository sectionRepository,
        IVenueRepository venueRepository,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        IFileStorage fileStorage) : IRequestHandler<CreateSessionCommand, Guid>
    {
        public async Task<Guid> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await eventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (eventEntity is null)
                throw new NotFoundException("Event not found.");

            var section = await sectionRepository.GetByIdAsync(request.SectionId, cancellationToken);
            if (section is null || section.EventId != request.EventId)
                throw new NotFoundException("Section not found for this event.");

            if (request.VenueId.HasValue)
            {
                var venue = await venueRepository.GetByIdAsync(request.VenueId.Value, cancellationToken);
                if (venue is null || venue.EventId != request.EventId)
                    throw new NotFoundException("Venue not found for this event.");
            }

            var startUtc = ToUtc(request.StartTime, eventEntity.TimeZone);
            var endUtc = ToUtc(request.EndTime, eventEntity.TimeZone);
            ValidateSchedule(startUtc, endUtc, eventEntity);

            if (request.VenueId.HasValue && startUtc.HasValue && endUtc.HasValue)
            {
                var existing = await sessionRepository.GetByEventIdAsync(request.EventId, cancellationToken);
                var conflict = existing.Any(x => x.VenueId == request.VenueId &&
                    x.StartTimeUtc.HasValue && x.EndTimeUtc.HasValue &&
                    startUtc < x.EndTimeUtc && endUtc > x.StartTimeUtc);
                if (conflict)
                    throw new ValidationException(new[] { new ValidationFailure("VenueId", "This venue is already assigned to another session during the selected time.") });
            }

            string? imageUrl = null;
            if (request.Image is not null)
            {
                var stored = await fileStorage.SaveAsync(request.Image, $"events/{request.EventId:D}/sessions", cancellationToken);
                imageUrl = stored.Url;
            }

            var session = new Session(request.EventId, request.SectionId, request.Title, request.Description, request.SessionType, request.Capacity, startUtc, endUtc, request.VenueId, imageUrl);
            await sessionRepository.AddAsync(session, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return session.Id;
        }

        private static DateTime? ToUtc(DateTime? value, string timeZoneId)
        {
            if (!value.HasValue) return null;
            var timeZone = TimeZoneHelper.GetTimeZone(timeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified), timeZone);
        }

        private static void ValidateSchedule(DateTime? start, DateTime? end, EventEntity eventEntity)
        {
            if (start.HasValue && start.Value < eventEntity.StartDate)
                throw new ValidationException(new[] { new ValidationFailure("StartTime", "Session cannot start before the event starts.") });
            if (end.HasValue && end.Value > eventEntity.EndDate)
                throw new ValidationException(new[] { new ValidationFailure("EndTime", "Session cannot end after the event ends.") });
        }
    }
}
