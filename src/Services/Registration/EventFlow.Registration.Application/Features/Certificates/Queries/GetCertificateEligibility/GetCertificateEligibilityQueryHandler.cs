using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Certificates;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateEligibility;

public sealed class GetCertificateEligibilityQueryHandler(
    ICertificateSettingsRepository settingsRepository,
    ICertificateRepository certificates,
    IRegistrationRepository registrations,
    ICertificateSourceDataService sourceData,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<GetCertificateEligibilityQuery, CertificateEligibilityDto>
{
    public async Task<CertificateEligibilityDto> Handle(
        GetCertificateEligibilityQuery query,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var settings = await settingsRepository.GetByEventIdAsync(
            query.EventId,
            cancellationToken);

        var requireApproved = settings?.RequireApprovedRegistration ?? true;
        var minAttendance = settings?.MinAttendancePercent;

        var registrationRows = await registrations.GetWithParticipantsAsync(
            query.EventId,
            cancellationToken);

        var issuedRegistrationIds = await certificates.GetIssuedRegistrationIdsAsync(
            query.EventId,
            cancellationToken);

        var attendanceCache = new Dictionary<Guid, double?>();
        var dto = new CertificateEligibilityDto();

        foreach (var registration in registrationRows)
        {
            var item = new CertificateEligibilityItemDto
            {
                RegistrationId = registration.Id,
                UserId = registration.UserId,
                RegistrationNumber = registration.RegistrationNumber,
                RegistrationStatus = registration.Status.ToString(),
                AttendancePercent = null
            };

            var participant = registration.Participant;

            if (participant is null)
            {
                item.ParticipantName = registration.RegistrationNumber;
            }
            else
            {
                item.ParticipantName =
                    $"{participant.FirstName} {participant.LastName}".Trim();
                item.Email = participant.Email;
            }

            double? attendance = null;
            if (minAttendance.HasValue)
            {
                if (!attendanceCache.TryGetValue(registration.UserId, out attendance))
                {
                    attendance = await sourceData.GetAttendancePercentAsync(
                        query.EventId,
                        registration.UserId,
                        cancellationToken);
                    attendanceCache[registration.UserId] = attendance;
                }

                item.AttendancePercent = attendance;
            }

            item.Reasons.AddRange(
                CertificateRules.Evaluate(
                    requireApproved,
                    minAttendance,
                    registration.Status,
                    participant is not null,
                    attendance));

            item.AlreadyIssued = issuedRegistrationIds.Contains(registration.Id);
            item.Eligible = !item.AlreadyIssued && item.Reasons.Count == 0;

            dto.TotalRegistrations++;
            if (item.AlreadyIssued) dto.AlreadyIssuedCount++;
            if (item.Eligible) dto.EligibleCount++;
            if (!item.Eligible && !item.AlreadyIssued) dto.IneligibleCount++;

            dto.Items.Add(item);
        }

        return dto;
    }
}
