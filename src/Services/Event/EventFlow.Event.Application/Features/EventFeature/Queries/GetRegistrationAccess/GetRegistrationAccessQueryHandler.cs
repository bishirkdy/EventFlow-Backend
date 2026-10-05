using EventFlow.Security.Authorization;
using MediatR;
using EventAuthorizationService = EventFlow.Event.Application.Abstractions.Authorization.IAuthorizationService;

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetRegistrationAccess;

public sealed class GetRegistrationAccessQueryHandler(EventAuthorizationService permissions)
    : IRequestHandler<GetRegistrationAccessQuery, bool>
{
    public async Task<bool> Handle(
        GetRegistrationAccessQuery request,
        CancellationToken cancellationToken)
    {
        return await permissions.HasPermissionAsync(
            request.UserId,
            request.EventId,
            PermissionConstants.Event.Update,
            cancellationToken);
    }
}
