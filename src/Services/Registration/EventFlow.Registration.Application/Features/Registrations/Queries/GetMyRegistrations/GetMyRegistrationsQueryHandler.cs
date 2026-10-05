using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetMyRegistrations;

public sealed class GetMyRegistrationsQueryHandler(
    IRegistrationRepository registrations,
    EventFlow.Security.Authentication.ICurrentUserService user)
    : IRequestHandler<
        GetMyRegistrationsQuery,
        IReadOnlyList<RegistrationDto>>
{
    public async Task<IReadOnlyList<RegistrationDto>> Handle(
        GetMyRegistrationsQuery query,
        CancellationToken cancellationToken)
    {
        var items = await registrations.GetForUserAsync(
            user.UserId,
            query.EventId,
            cancellationToken);

        return items.Select(x => x.ToDto()).ToList();
    }
}
