using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetParticipantCount;

public sealed record GetParticipantCountQuery : IRequest<int>;
