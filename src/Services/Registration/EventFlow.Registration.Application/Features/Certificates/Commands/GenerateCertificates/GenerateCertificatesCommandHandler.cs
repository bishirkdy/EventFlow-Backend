using System.Security.Cryptography;
using EventFlow.Registration.Application.Common.Certificates;
using EventFlow.SharedKernel.Exceptions;
using EventFlow.Security.Authentication;
using Microsoft.Extensions.Logging;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.GenerateCertificates;

public sealed class GenerateCertificatesCommandHandler(
    ICertificateRepository certificates,
    ICertificateSettingsRepository settingsRepository,
    IRegistrationRepository registrations,
    ICertificateSourceDataService sourceData,
    ICertificateDocumentService documentService,
    ICertificateFileStore fileStore,
    ICertificateVerifyUrlProvider verifyUrlProvider,
    ICertificateTemplateBackgroundResolver templateBackground,
    IUnitOfWork unitOfWork,
    ICurrentUserService user,
    IEventRegistrationAccessService access,
    ILogger<GenerateCertificatesCommandHandler> logger)
    : IRequestHandler<GenerateCertificatesCommand, CertificateGenerationResultDto>
{
    private const int MaxReportedErrors = 20;

    public async Task<CertificateGenerationResultDto> Handle(
        GenerateCertificatesCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(command.EventId,user.UserId,cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var settings = await settingsRepository.GetByEventIdAsync(command.EventId, cancellationToken);

        var requireApproved = settings?.RequireApprovedRegistration ?? true;
        var minAttendance = settings?.MinAttendancePercent;

        var eventInfo = await sourceData.GetEventAsync(command.EventId, cancellationToken);

        if (eventInfo is null)
        {
            throw new NotFoundException("Event details could not be loaded.");
        }

        var registrationRows = await registrations.GetWithParticipantsAsync(command.EventId,cancellationToken);

        var issuedRegistrationIds = await certificates.GetIssuedRegistrationIdsAsync(command.EventId, cancellationToken);

        var requestedIds = command.Request.RegistrationIds;

        var candidates = requestedIds is { Count: > 0 }
            ? registrationRows.Where(x => requestedIds.Contains(x.Id))
            : registrationRows.AsEnumerable();

        var result = new CertificateGenerationResultDto();
        var settingsSnapshot = settings ?? CreateDefaultSettings(command.EventId);
        var datesText = CertificateEventText.FormatEventDates(eventInfo.StartDate, eventInfo.EndDate);

        var useImageTemplate = settingsSnapshot.TemplateMode == CertificateTemplateMode.ImageTemplate;

        byte[]? backgroundImage = null;
        if (useImageTemplate)
        {
            backgroundImage = await templateBackground.ResolveAsync(settingsSnapshot, cancellationToken);
        }

        var attendanceCache = new Dictionary<Guid, double?>();
        var savedFiles = new List<string>();

        foreach (var registration in candidates)
        {
            if (issuedRegistrationIds.Contains(registration.Id))
            {
                result.Skipped++;
                continue;
            }

            var participant = registration.Participant;

            double? attendance = null;
            if (minAttendance.HasValue)
            {
                if (!attendanceCache.TryGetValue(registration.UserId, out attendance))
                {
                    attendance = await sourceData.GetAttendancePercentAsync(
                        command.EventId,
                        registration.UserId,
                        cancellationToken);

                    attendanceCache[registration.UserId] = attendance;
                }
            }

            var reasons = CertificateRules.Evaluate(
                requireApproved,
                minAttendance,
                registration.Status,
                participant is not null,
                attendance);

            if (reasons.Count > 0)
            {
                result.Skipped++;
                continue;
            }

            var certificateNumber = await GenerateUniqueNumberAsync(
                cancellationToken);

            var fileName = $"{certificateNumber}.pdf";

            var documentData = new CertificateDocumentData(
                certificateNumber,
                participant is null
                    ? registration.RegistrationNumber
                    : $"{participant.FirstName} {participant.LastName}".Trim(),
                eventInfo.Name,
                datesText,
                settingsSnapshot.Title,
                settingsSnapshot.Subtitle,
                settingsSnapshot.ThemeColor,
                settingsSnapshot.SignatoryName,
                settingsSnapshot.SignatoryTitle,
                DateTime.UtcNow,
                verifyUrlProvider.GetUrl(certificateNumber),
                useImageTemplate,
                settingsSnapshot.TemplateFields,
                backgroundImage,
                settingsSnapshot.BackgroundImageWidth,
                settingsSnapshot.BackgroundImageHeight);

            byte[] pdf;
            try
            {
                pdf = documentService.Generate(documentData);
                await fileStore.SaveAsync(
                    command.EventId,
                    fileName,
                    pdf,
                    cancellationToken);

                savedFiles.Add(fileName);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to generate certificate for registration {RegistrationId} in event {EventId}.",
                    registration.Id,
                    command.EventId);

                result.Failed++;

                if (result.Errors.Count < MaxReportedErrors)
                {
                    result.Errors.Add(
                        $"{registration.RegistrationNumber}: {DescribeFailure(ex)}");
                }

                continue;
            }

            var certificate = new Certificate
            {
                Id = Guid.NewGuid(),
                EventId = command.EventId,
                RegistrationId = registration.Id,
                ParticipantId = participant?.Id ?? Guid.Empty,
                UserId = registration.UserId,
                CertificateNumber = certificateNumber,
                ParticipantName = documentData.ParticipantName,
                ParticipantEmail = participant?.Email ?? "",
                EventName = eventInfo.Name,
                DocumentFileName = fileName,
                IssuedAtUtc = documentData.IssuedAtUtc,
                GeneratedByUserId = user.UserId
            };

            certificates.Add(certificate);
            issuedRegistrationIds.Add(registration.Id);

            result.Generated++;
            result.Certificates.Add(certificate.ToDto());
        }

        if (result.Generated > 0)
        {
            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to persist {Count} generated certificate(s) for event {EventId}; removing written files.",
                    savedFiles.Count,
                    command.EventId);

                await DeleteSavedFilesAsync(command.EventId, savedFiles);

                throw;
            }
        }

        return result;
    }

    private async Task DeleteSavedFilesAsync(Guid eventId, IReadOnlyList<string> fileNames)
    {
        foreach (var fileName in fileNames)
        {
            try
            {
                await fileStore.DeleteAsync(eventId, fileName);
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "Failed to clean up certificate file {FileName} for event {EventId}.",
                    fileName,
                    eventId);
            }
        }
    }

    private static string DescribeFailure(Exception exception) => exception switch
    {
        EventFlowException flowException => flowException.Message,
        InvalidOperationException invalidOperation => invalidOperation.Message,
        _ => "Unexpected error while generating the certificate."
    };

    private async Task<string> GenerateUniqueNumberAsync(
        CancellationToken cancellationToken)
    {
        var prefix = $"CERT-{DateTime.UtcNow:yyyyMMdd}";

        for (var attempt = 0; attempt < 5; attempt++)
        {
            var suffix = Convert.ToHexString(
                RandomNumberGenerator.GetBytes(3))[..6];

            var number = $"{prefix}-{suffix}";

            if (await certificates.GetByNumberAsync(number, cancellationToken) is null)
            {
                return number;
            }
        }

        return $"{prefix}-{Guid.NewGuid():N}"[..22];
    }

    private static CertificateSettings CreateDefaultSettings(Guid eventId) => new()
    {
        Id = Guid.NewGuid(),
        EventId = eventId,
        CreatedAtUtc = DateTime.UtcNow,
        UpdatedAtUtc = DateTime.UtcNow
    };
}
