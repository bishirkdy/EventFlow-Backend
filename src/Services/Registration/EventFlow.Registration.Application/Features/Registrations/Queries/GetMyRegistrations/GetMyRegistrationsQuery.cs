using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetMyRegistrations;

public sealed record GetMyRegistrationsQuery(
    Guid? EventId = null)
    : IRequest<IReadOnlyList<RegistrationDto>>;
