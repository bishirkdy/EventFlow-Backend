using AutoMapper;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetMyRegistrations;

public sealed class GetMyRegistrationsQueryHandler(
    IRegistrationRepository registrations,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IMapper mapper)
    : IRequestHandler<GetMyRegistrationsQuery, IReadOnlyList<RegistrationDto>>
{
    public async Task<IReadOnlyList<RegistrationDto>> Handle(
        GetMyRegistrationsQuery query,
        CancellationToken cancellationToken)
    {
        var items = await registrations.GetForUserAsync(
            user.UserId,
            query.EventId,
            cancellationToken);

        return mapper.Map<List<RegistrationDto>>(items);
    }
}
