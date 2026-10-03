using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Domain.Enums;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateEligibility;

public sealed class GetCertificateEligibilityQueryHandler(
    ICertificateSettingsRepository settingsRepository,
    ICertificateRepository certificates,
    IRegistrationRepository registrations,
    ICertificateSourceDataService sourceData,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<GetCertificateEligibilityQuery, ApiResponse<CertificateEligibilityDto>>
{
    public async Task<ApiResponse<CertificateEligibilityDto>> Handle(
        GetCertificateEligibilityQuery query,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<CertificateEligibilityDto>.Fail(
                ["You do not have permission."]);
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
                item.Reasons.Add("No participant profile.");
            }
            else
            {
                item.ParticipantName =
                    $"{participant.FirstName} {participant.LastName}".Trim();
                item.Email = participant.Email;
            }

            if (registration.Status is
                RegistrationStatus.Cancelled or RegistrationStatus.Rejected)
            {
                item.Reasons.Add(
                    registration.Status == RegistrationStatus.Cancelled
                        ? "Registration was cancelled."
                        : "Registration was rejected.");
            }
            else if (requireApproved &&
                     registration.Status != RegistrationStatus.Approved)
            {
                item.Reasons.Add("Registration is not approved.");
            }

            if (minAttendance.HasValue)
            {
                if (!attendanceCache.TryGetValue(registration.UserId, out var attendance))
                {
                    attendance = await sourceData.GetAttendancePercentAsync(
                        query.EventId,
                        registration.UserId,
                        cancellationToken);
                    attendanceCache[registration.UserId] = attendance;
                }

                item.AttendancePercent = attendance;

                if (attendance is null)
                {
                    item.Reasons.Add("Attendance could not be verified.");
                }
                else if (attendance.Value < minAttendance.Value)
                {
                    item.Reasons.Add(
                        $"Attendance {attendance.Value:0.#}% is below the required {minAttendance.Value:0.#}%.");
                }
            }

            item.AlreadyIssued = issuedRegistrationIds.Contains(registration.Id);
            item.Eligible = !item.AlreadyIssued && item.Reasons.Count == 0;

            dto.TotalRegistrations++;
            if (item.AlreadyIssued) dto.AlreadyIssuedCount++;
            if (item.Eligible) dto.EligibleCount++;
            if (!item.Eligible && !item.AlreadyIssued) dto.IneligibleCount++;

            dto.Items.Add(item);
        }

        return ApiResponse<CertificateEligibilityDto>.Success(
            dto,
            "Eligibility computed.");
    }
}
