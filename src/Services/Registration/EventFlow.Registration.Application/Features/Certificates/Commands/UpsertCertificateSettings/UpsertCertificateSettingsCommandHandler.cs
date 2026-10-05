using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Domain.Entities;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.UpsertCertificateSettings;

public sealed class UpsertCertificateSettingsCommandHandler(
    ICertificateSettingsRepository settings,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<UpsertCertificateSettingsCommand, CertificateSettingsDto>
{
    public async Task<CertificateSettingsDto> Handle(
        UpsertCertificateSettingsCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var request = command.Request;

        var entity = await settings.GetByEventIdAsync(
            command.EventId,
            cancellationToken);

        var now = DateTime.UtcNow;

        if (entity is null)
        {
            entity = new CertificateSettings
            {
                Id = Guid.NewGuid(),
                EventId = command.EventId,
                CreatedAtUtc = now
            };
            settings.Add(entity);
        }

        entity.Title = request.Title.Trim();
        entity.Subtitle = request.Subtitle.Trim();
        entity.SignatoryName = NullIfWhiteSpace(request.SignatoryName);
        entity.SignatoryTitle = NullIfWhiteSpace(request.SignatoryTitle);
        entity.ThemeColor =
            string.IsNullOrWhiteSpace(request.ThemeColor)
                ? "#2563EB"
                : request.ThemeColor.Trim();
        entity.RequireApprovedRegistration = request.RequireApprovedRegistration;
        entity.MinAttendancePercent = request.MinAttendancePercent;
        entity.UpdatedAtUtc = now;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(entity);
    }

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static CertificateSettingsDto ToDto(CertificateSettings entity) => new()
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
    };
}
