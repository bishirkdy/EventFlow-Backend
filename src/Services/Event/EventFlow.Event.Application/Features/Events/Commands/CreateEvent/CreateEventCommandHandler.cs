using EventFlow.Event.Application.Abstractions.Authentication;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Services;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Exceptions;
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
        private readonly IUserDirectoryClient _userDirectoryClient;
        private readonly IEventSettingsRepository _eventSettingsRepository;
        private readonly IEventTypeRepository _eventTypeRepository;
        private readonly ILogger<CreateEventCommandHandler> _logger;

        public CreateEventCommandHandler(
            IEventRepository eventRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IFileStorage fileStorage,
            IUserDirectoryClient userDirectoryClient,
            IEventSettingsRepository eventSettingsRepository,
            IEventTypeRepository eventTypeRepository,
            ILogger<CreateEventCommandHandler> logger)
        {
            _eventRepository = eventRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileStorage = fileStorage;
            _userDirectoryClient = userDirectoryClient;
            _logger = logger;
            _eventSettingsRepository = eventSettingsRepository;
            _eventTypeRepository = eventTypeRepository;
        }

        public async Task<CreateEventResult> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {

            var timeZone = TimeZoneHelper.GetTimeZone(request.TimeZone);

            var startDate = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(request.StartDate, DateTimeKind.Unspecified),timeZone);

            var endDate = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(request.EndDate, DateTimeKind.Unspecified),timeZone);

            var eventType = await _eventTypeRepository.GetByIdAsync(request.EventTypeId,cancellationToken);

            if (eventType is null || !eventType.IsActive)
            {
                throw new NotFoundException("Event type not found.");
            }

            var eventEntity = new EventEntity(
                request.Name,
                request.Description,
                request.EventTypeId,
                request.SubType,
                startDate,
                endDate,
                request.TimeZone,
                _currentUserService.UserId);

            var eventSettings = new EventFlow.Event.Domain.Entities.EventSettings(eventEntity.Id);

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
                await _eventSettingsRepository.AddAsync(eventSettings, cancellationToken);
                await _eventRepository.AddAsync(eventEntity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var createdByName = await _userDirectoryClient.GetDisplayNameAsync(
                    eventEntity.CreatedBy,
                    cancellationToken);

                return new CreateEventResult(
                    eventEntity.Id,
                    eventEntity.Name,
                    eventEntity.Description,
                    eventEntity.EventTypeId,
                    eventEntity.SubType,
                    eventEntity.StartDate,
                    eventEntity.EndDate,
                    eventEntity.TimeZone,
                    eventEntity.Status.ToString(),
                    eventEntity.Subdomain,
                    eventEntity.CreatedBy,
                    createdByName,
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
