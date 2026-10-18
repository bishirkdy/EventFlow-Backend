
using AutoMapper;
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
        var form = await forms.GetByEventIdAsync(query.EventId, includeFields: true, asNoTracking: true, cancellationToken: cancellationToken);

        if (form is null)
        {
            // Events get a ready to use registration form the first time
            // somebody opens it, so registration never starts empty handed.
            await CreateDefaultFormAsync(query.EventId, cancellationToken);

            form = await forms.GetByEventIdAsync(query.EventId, includeFields: true, asNoTracking: true, cancellationToken: cancellationToken);

            if (form is null)
            {
                throw new NotFoundException("Registration form not found.");
            }
        }

        var approvedCount = await registrations.CountByStatusAsync(query.EventId, RegistrationStatus.Approved, cancellationToken);

        var waitlistCount = await registrations.CountByStatusAsync(query.EventId, RegistrationStatus.Waitlisted, cancellationToken);

        var response = mapper.Map<RegistrationFormDto>(form);
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
