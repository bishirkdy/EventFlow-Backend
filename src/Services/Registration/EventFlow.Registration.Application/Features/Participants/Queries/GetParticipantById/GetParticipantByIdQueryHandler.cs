using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

namespace EventFlow.Registration.Application.Features.Participants.Queries.GetParticipantById;

public sealed class GetParticipantByIdQueryHandler(
    IParticipantRepository participants,
    ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<GetParticipantByIdQuery, ApiResponse<ParticipantDto>>
{
    public async Task<ApiResponse<ParticipantDto>> Handle(
        GetParticipantByIdQuery query,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<ParticipantDto>.Fail(
                ["You do not have permission."]);
        }

        var participant = await participants.GetByIdAsync(
            query.EventId,
            query.ParticipantId,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        return participant is null
            ? ApiResponse<ParticipantDto>.Fail(["Participant not found."])
            : ApiResponse<ParticipantDto>.Success(participant.ToDto());
    }
}
