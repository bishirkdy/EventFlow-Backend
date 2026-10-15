using AutoMapper;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.UpsertCertificateSettings;

public sealed class UpsertCertificateSettingsCommandHandler(
    ICertificateSettingsRepository settings,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access,
    IMapper mapper)
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

        if (entity is null)
        {
            entity = new CertificateSettings(command.EventId);
            settings.Add(entity);
        }

        entity.Update(
            request.Title.Trim(),
            request.Subtitle.Trim(),
            NullIfWhiteSpace(request.SignatoryName),
            NullIfWhiteSpace(request.SignatoryTitle),
            string.IsNullOrWhiteSpace(request.ThemeColor)
                ? "#2563EB"
                : request.ThemeColor.Trim(),
            request.RequireApprovedRegistration,
            request.MinAttendancePercent,
            mapper.Map<CertificateTemplateMode>(request.TemplateMode),
            request.TemplateFields
                .Select(mapper.Map<CertificateTemplateField>)
                .ToList());

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<CertificateSettingsDto>(entity);
    }

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
