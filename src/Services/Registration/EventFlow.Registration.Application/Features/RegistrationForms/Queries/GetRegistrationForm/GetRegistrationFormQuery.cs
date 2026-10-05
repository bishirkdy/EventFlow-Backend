namespace EventFlow.Registration.Application.Features.RegistrationForms.Queries.GetRegistrationForm;

public sealed record GetRegistrationFormQuery(Guid EventId) : IRequest<RegistrationFormDto>;
