using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateSettings;

public sealed class GetCertificateSettingsQueryHandler(
    ICertificateSettingsRepository settings,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<GetCertificateSettingsQuery, ApiResponse<CertificateSettingsDto>>
{
    public async Task<ApiResponse<CertificateSettingsDto>> Handle(
        GetCertificateSettingsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<CertificateSettingsDto>.Fail(
                ["You do not have permission."]);
        }

        var entity = await settings.GetByEventIdAsync(
            query.EventId,
            cancellationToken);

        if (entity is null)
        {
            return ApiResponse<CertificateSettingsDto>.Success(
                new CertificateSettingsDto
                {
                    EventId = query.EventId,
                    Title = "Certificate of Participation",
                    Subtitle = "",
                    ThemeColor = "#2563EB",
                    RequireApprovedRegistration = true
                },
                "Using default certificate settings.");
        }

        return ApiResponse<CertificateSettingsDto>.Success(
            new CertificateSettingsDto
            {
                EventId = entity.EventId,
                Title = entity.Title,
                Subtitle = entity.Subtitle,
                SignatoryName = entity.SignatoryName,
                SignatoryTitle = entity.SignatoryTitle,
                ThemeColor = entity.ThemeColor,
                RequireApprovedRegistration = entity.RequireApprovedRegistration,
                MinAttendancePercent = entity.MinAttendancePercent,
                UpdatedAtUtc = entity.UpdatedAtUtc
            },
            "Certificate settings retrieved.");
    }
}
