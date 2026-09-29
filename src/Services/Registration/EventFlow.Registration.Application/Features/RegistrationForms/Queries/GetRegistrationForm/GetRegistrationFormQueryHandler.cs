using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Contracts.RegistrationForms;
using EventFlow.Registration.Domain.Enums;
using MediatR;

namespace EventFlow.Registration.Application.Features.RegistrationForms.Queries.GetRegistrationForm;

public sealed class GetRegistrationFormQueryHandler(
    IRegistrationFormRepository forms,
    IRegistrationRepository registrations)
    : IRequestHandler<
        GetRegistrationFormQuery,
        ApiResponse<RegistrationFormDto>>
{
    public async Task<ApiResponse<RegistrationFormDto>> Handle(
        GetRegistrationFormQuery query,
        CancellationToken cancellationToken)
    {
        var form = await forms.GetByEventIdAsync(
            query.EventId,
            includeFields: true,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        if (form is null)
        {
            return ApiResponse<RegistrationFormDto>.Fail(
                ["Registration form not found."]);
        }

        var approvedCount = await registrations.CountByStatusAsync(
            query.EventId,
            RegistrationStatus.Approved,
            cancellationToken);

        var waitlistCount = await registrations.CountByStatusAsync(
            query.EventId,
            RegistrationStatus.Waitlisted,
            cancellationToken);

        var response = new RegistrationFormDto
        {
            Id = form.Id,
            EventId = form.EventId,
            Name = form.Name,
            Description = form.Description,
            IsActive = form.IsActive,
            CapacityMode = form.CapacityMode,
            Capacity = form.Capacity,
            ApprovedCount = approvedCount,
            WaitlistCount = waitlistCount,
            EnableWaitlist = form.EnableWaitlist,
            OpensAtUtc = form.OpensAtUtc,
            ClosesAtUtc = form.ClosesAtUtc,
            Fields = form.Fields
                .OrderBy(x => x.DisplayOrder)
                .Select(
                    x => new RegistrationFormFieldDto
                    {
                        Id = x.Id,
                        FieldKey = x.FieldKey,
                        Label = x.Label,
                        FieldType = x.FieldType,
                        IsRequired = x.IsRequired,
                        DisplayOrder = x.DisplayOrder,
                        OptionsJson = x.OptionsJson,
                        ValidationJson = x.ValidationJson
                    })
                .ToList()
        };

        return ApiResponse<RegistrationFormDto>.Success(response);
    }
}
