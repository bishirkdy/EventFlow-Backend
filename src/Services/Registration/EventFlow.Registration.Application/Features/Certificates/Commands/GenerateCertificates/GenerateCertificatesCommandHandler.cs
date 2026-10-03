using System.Globalization;
using System.Security.Cryptography;
using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Certificates;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Domain.Entities;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.GenerateCertificates;

public sealed class GenerateCertificatesCommandHandler(
    ICertificateRepository certificates,
    ICertificateSettingsRepository settingsRepository,
    IRegistrationRepository registrations,
    ICertificateSourceDataService sourceData,
    ICertificateDocumentService documentService,
    ICertificateFileStore fileStore,
    ICertificateVerifyUrlProvider verifyUrlProvider,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<GenerateCertificatesCommand, ApiResponse<CertificateGenerationResultDto>>
{
    public async Task<ApiResponse<CertificateGenerationResultDto>> Handle(
        GenerateCertificatesCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<CertificateGenerationResultDto>.Fail(
                ["You do not have permission."]);
        }

        var settings = await settingsRepository.GetByEventIdAsync(
            command.EventId,
            cancellationToken);

        var requireApproved = settings?.RequireApprovedRegistration ?? true;
        var minAttendance = settings?.MinAttendancePercent;

        var eventInfo = await sourceData.GetEventAsync(
            command.EventId,
            cancellationToken);

        if (eventInfo is null)
        {
            return ApiResponse<CertificateGenerationResultDto>.Fail(
                ["Event details could not be loaded."]);
        }

        var registrationRows = await registrations.GetWithParticipantsAsync(
            command.EventId,
            cancellationToken);

        var issuedRegistrationIds = await certificates.GetIssuedRegistrationIdsAsync(
            command.EventId,
            cancellationToken);

        var requestedIds = command.Request.RegistrationIds;

        var candidates = requestedIds is { Count: > 0 }
            ? registrationRows.Where(x => requestedIds.Contains(x.Id))
            : registrationRows.AsEnumerable();

        var result = new CertificateGenerationResultDto();
        var settingsSnapshot = settings ?? CreateDefaultSettings(command.EventId);
        var datesText = FormatEventDates(eventInfo.StartDate, eventInfo.EndDate);

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
                attendance = await sourceData.GetAttendancePercentAsync(
                    command.EventId,
                    registration.UserId,
                    cancellationToken);
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
                verifyUrlProvider.GetUrl(certificateNumber));

            byte[] pdf;
            try
            {
                pdf = documentService.Generate(documentData);
                await fileStore.SaveAsync(
                    command.EventId,
                    fileName,
                    pdf,
                    cancellationToken);
            }
            catch
            {
                result.Failed++;
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
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var message = result.Failed > 0
            ? $"Generated {result.Generated} certificate(s), {result.Skipped} skipped, {result.Failed} failed."
            : $"Generated {result.Generated} certificate(s), {result.Skipped} skipped.";

        return ApiResponse<CertificateGenerationResultDto>.Success(result, message);
    }

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

    private static string FormatEventDates(DateTime startDate, DateTime endDate)
    {
        if (startDate.Date == endDate.Date)
        {
            return startDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
        }

        if (startDate.Year == endDate.Year &&
            startDate.Month == endDate.Month)
        {
            return $"{startDate.Day} - {endDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture)}";
        }

        if (startDate.Year == endDate.Year)
        {
            return $"{startDate.ToString("d MMM", CultureInfo.InvariantCulture)} - {endDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture)}";
        }

        return $"{startDate.ToString("d MMM yyyy", CultureInfo.InvariantCulture)} - {endDate.ToString("d MMM yyyy", CultureInfo.InvariantCulture)}";
    }
}
