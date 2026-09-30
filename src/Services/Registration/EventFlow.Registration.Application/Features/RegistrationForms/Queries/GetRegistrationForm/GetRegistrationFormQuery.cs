using EventFlow.Contracts.Common;

namespace EventFlow.Registration.Application.Features.RegistrationForms.Queries.GetRegistrationForm;

public sealed record GetRegistrationFormQuery(Guid EventId) : IRequest<ApiResponse<RegistrationFormDto>>;
