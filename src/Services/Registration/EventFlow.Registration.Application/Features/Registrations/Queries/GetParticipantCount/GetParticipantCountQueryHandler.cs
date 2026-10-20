using EventFlow.Registration.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetParticipantCount;

public sealed class GetParticipantCountQueryHandler(
    IRegistrationRepository registrations)
    : IRequestHandler<GetParticipantCountQuery, int>
{
    public Task<int> Handle(
        GetParticipantCountQuery query,
        CancellationToken cancellationToken)
    {
        return registrations.CountParticipantsAsync(cancellationToken);
    }
}
