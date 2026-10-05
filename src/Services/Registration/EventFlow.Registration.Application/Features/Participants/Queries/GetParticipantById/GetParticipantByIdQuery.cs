namespace EventFlow.Registration.Application.Features.Participants.Queries.GetParticipantById;

public sealed record GetParticipantByIdQuery(Guid EventId, Guid ParticipantId) : IRequest<ParticipantDto>;
