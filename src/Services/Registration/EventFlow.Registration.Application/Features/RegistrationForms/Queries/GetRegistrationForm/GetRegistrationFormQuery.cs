using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Application.Features.RegistrationForms.Queries.GetRegistrationForm;

public sealed record GetRegistrationFormQuery(
    Guid EventId,
    RegistrationScope ScopeType = RegistrationScope.Event,
    Guid? ScopeId = null) : IRequest<RegistrationFormDto>;
