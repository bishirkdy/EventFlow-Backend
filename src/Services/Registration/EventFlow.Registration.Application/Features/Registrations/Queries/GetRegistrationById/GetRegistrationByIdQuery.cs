using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationById;

public sealed record GetRegistrationByIdQuery(
    Guid EventId,
    Guid RegistrationId,
    bool OrganizerView = false
) : IRequest<RegistrationDto>;
