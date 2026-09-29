using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

using EventFlow.Contracts.Common;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationById;

public sealed record GetRegistrationByIdQuery(
    Guid EventId,
    Guid RegistrationId,
    bool OrganizerView = false
) : IRequest<ApiResponse<RegistrationDto>>;
