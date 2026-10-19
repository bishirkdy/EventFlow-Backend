
using AutoMapper;
using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Exceptions;

namespace EventFlow.Registration.Application.Features.RegistrationForms.Queries.GetRegistrationForm;

public sealed class GetRegistrationFormQueryHandler(
    IRegistrationFormRepository forms,
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetRegistrationFormQuery, RegistrationFormDto>
{
    public async Task<RegistrationFormDto> Handle(GetRegistrationFormQuery query, CancellationToken cancellationToken)
    {
        var scopeType = query.ScopeType;
        var scopeId = scopeType == RegistrationScope.Session ? query.ScopeId : null;

        var isInherited = false;

        var form = await forms.GetByScopeAsync(
            query.EventId,
            scopeType,
            scopeId,
            includeFields: true,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        if (form is null && scopeType == RegistrationScope.Session)
        {
            // A session without its own form simply inherits the event form.
            form = await forms.GetByEventIdAsync(
                query.EventId, includeFields: true, asNoTracking: true, cancellationToken: cancellationToken);

            isInherited = true;

            if (form is null)
            {
                await CreateDefaultFormAsync(query.EventId, cancellationToken);

                form = await forms.GetByEventIdAsync(
                    query.EventId, includeFields: true, asNoTracking: true, cancellationToken: cancellationToken);
            }
        }
        else if (form is null)
        {
            // Events get a ready to use registration form the first time
            // somebody opens it, so registration never starts empty handed.
            await CreateDefaultFormAsync(query.EventId, cancellationToken);

            form = await forms.GetByEventIdAsync(
                query.EventId, includeFields: true, asNoTracking: true, cancellationToken: cancellationToken);
        }

        if (form is null)
        {
            throw new NotFoundException("Registration form not found.");
        }

        var approvedCount = await registrations.CountByScopeAsync(
            query.EventId, scopeType, scopeId, RegistrationStatus.Approved, cancellationToken);

        var waitlistCount = await registrations.CountByScopeAsync(
            query.EventId, scopeType, scopeId, RegistrationStatus.Waitlisted, cancellationToken);

        var response = mapper.Map<RegistrationFormDto>(form);
        response.IsInherited = isInherited;
        response.ApprovedCount = approvedCount;
        response.WaitlistCount = waitlistCount;

        return response;
    }

    private async Task CreateDefaultFormAsync(Guid eventId, CancellationToken cancellationToken)
    {
        var form = new RegistrationForm(eventId);

        forms.Add(form);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            // Another request created the form at the same time
            // (EventId is unique), so simply keep using that one.
            var existing = await forms.GetByEventIdAsync(eventId, cancellationToken: cancellationToken);

            if (existing is null)
            {
                throw;
            }
        }
    }
}