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

namespace EventFlow.Event.Application.Features.Events.Commands.CreateEvent;

public sealed class CreateEventCommandHandler
    : IRequestHandler<CreateEventCommand, CreateEventResult>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorage _fileStorage;
    private readonly IUserDirectoryClient _userDirectoryClient;
    private readonly IEventSettingsRepository _eventSettingsRepository;
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IEventFeatureRepository _eventFeatureRepository;
    private readonly IEventTypeFeatureRepository _eventTypeFeatureRepository;
    private readonly ILogger<CreateEventCommandHandler> _logger;

    public CreateEventCommandHandler(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IFileStorage fileStorage,
        IUserDirectoryClient userDirectoryClient,
        IEventSettingsRepository eventSettingsRepository,
        IEventTypeRepository eventTypeRepository,
        IEventTypeFeatureRepository eventTypeFeatureRepository,
        IEventFeatureRepository eventFeatureRepository,
        ILogger<CreateEventCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fileStorage = fileStorage;
        _userDirectoryClient = userDirectoryClient;
        _eventSettingsRepository = eventSettingsRepository;
        _eventTypeRepository = eventTypeRepository;
        _eventTypeFeatureRepository = eventTypeFeatureRepository;
        _eventFeatureRepository = eventFeatureRepository;
        _logger = logger;
    }

    public async Task<CreateEventResult> Handle(
        CreateEventCommand request,
        CancellationToken cancellationToken)
    {
        var timeZone = TimeZoneHelper.GetTimeZone(request.TimeZone);

        var startDate = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(
                request.StartDate,
                DateTimeKind.Unspecified),
            timeZone);

        var endDate = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(
                request.EndDate,
                DateTimeKind.Unspecified),
            timeZone);

        // Validate event type
        var eventType = await _eventTypeRepository.GetByIdAsync(
            request.EventTypeId,
            cancellationToken);

        if (eventType is null || !eventType.IsActive)
        {
            throw new NotFoundException("Event type not found.");
        }

        // Create event
        var eventEntity = new EventEntity(
            request.Name,
            request.Description,
            request.EventTypeId,
            request.SubType,
            startDate,
            endDate,
            request.TimeZone,
            _currentUserService.UserId);

        // Create default event settings
        var eventSettings =
            new Domain.Entities.EventSettings(eventEntity.Id);

        // Get default features for this event type
        var eventTypeFeatures =
            await _eventTypeFeatureRepository.GetByEventTypeIdAsync(
                request.EventTypeId,
                cancellationToken);

        // Create event feature records
        foreach (var defaultFeature in eventTypeFeatures)
        {
            var eventFeature = new Domain.Entities.EventFeature(
                eventEntity.Id,
                defaultFeature.FeatureId,
                defaultFeature.IsEnabledByDefault);

            await _eventFeatureRepository.AddAsync(
                eventFeature,
                cancellationToken);
        }

        var storedFiles = new List<StoredFile>();

        try
        {
            // Store event images
            foreach (var (image, index) in request.Images.Select(
                (image, index) => (image, index)))
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

            // Add event settings
            await _eventSettingsRepository.AddAsync(
                eventSettings,
                cancellationToken);

            // Add event
            await _eventRepository.AddAsync(
                eventEntity,
                cancellationToken);

            // Save event + settings + features + images
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            // Get creator display name
            var createdByName =
                await _userDirectoryClient.GetDisplayNameAsync(
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
            // Remove uploaded files if database/event creation fails
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