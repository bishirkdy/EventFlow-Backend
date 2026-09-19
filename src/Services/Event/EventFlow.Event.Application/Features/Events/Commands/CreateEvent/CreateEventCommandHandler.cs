using EventFlow.Event.Application.Abstractions.Authentication;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Features.Events.Common;
using EventFlow.Event.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Event.Application.Features.Events.Commands.CreateEvent
{
    public sealed class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, CreateEventResult>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorage _fileStorage;
        private readonly ILogger<CreateEventCommandHandler> _logger;

        public CreateEventCommandHandler(
            IEventRepository eventRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IFileStorage fileStorage,
            ILogger<CreateEventCommandHandler> logger)
        {
            _eventRepository = eventRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileStorage = fileStorage;
            _logger = logger;
        }

        public async Task<CreateEventResult> Handle(
            CreateEventCommand request,
            CancellationToken cancellationToken)
        {
            var timeZone = TimeZoneHelper.GetTimeZone(request.TimeZone);

            var startDate = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(request.StartDate, DateTimeKind.Unspecified),
                timeZone);

            var endDate = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(request.EndDate, DateTimeKind.Unspecified),
                timeZone);

            var eventEntity = new EventEntity(
                request.Name,
                request.Description,
                request.EventType,
                request.SubType,
                startDate,
                endDate,
                request.TimeZone,
                _currentUserService.UserId);

            var storedFiles = new List<StoredFile>();

            try
            {
                foreach (var (image, index) in request.Images.Select((image, index) => (image, index)))
                {
                    var storedFile = await _fileStorage.SaveAsync(
                        image,
                        $"events/{eventEntity.Id:D}",
                        cancellationToken);

                    storedFiles.Add(storedFile);

                    eventEntity.AddImage(
                        new EventImage(
                            eventEntity.Id,
                            storedFile.Url,
                            storedFile.StorageKey,
                            storedFile.FileName,
                            storedFile.ContentType,
                            storedFile.Length,
                            index));
                }

                await _eventRepository.AddAsync(eventEntity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new CreateEventResult(
                    eventEntity.Id,
                    eventEntity.Name,
                    eventEntity.Description,
                    eventEntity.EventType,
                    eventEntity.SubType,
                    eventEntity.StartDate,
                    eventEntity.EndDate,
                    eventEntity.TimeZone,
                    eventEntity.Status.ToString(),
                    eventEntity.Subdomain,
                    eventEntity.CreatedBy,
                    eventEntity.CreatedAt,
                    eventEntity.UpdatedAt,
                    eventEntity.Images
                        .OrderBy(image => image.DisplayOrder)
                        .Select(image => new EventImageResponse(
                            image.Id,
                            image.Url,
                            image.OriginalFileName,
                            image.ContentType,
                            image.SizeBytes,
                            image.DisplayOrder))
                        .ToList());
            }
            catch
            {
                foreach (var storedFile in storedFiles.AsEnumerable().Reverse())
                {
                    try
                    {
                        await _fileStorage.DeleteAsync(
                            storedFile.StorageKey,
                            CancellationToken.None);
                    }
                    catch (Exception cleanupException)
                    {
                        _logger.LogError(
                            cleanupException,
                            "Failed to clean up event image {StorageKey} after event creation failed.",
                            storedFile.StorageKey);
                    }
                }

                throw;
            }
        }
    }
}
