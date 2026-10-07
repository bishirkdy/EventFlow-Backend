using EventFlow.Identity.Application.Abstractions.Authorization;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.CheckPermission;

public sealed class CheckPermissionQueryHandler(IPermissionService permissions)
    : IRequestHandler<CheckPermissionQuery, bool>
{
    public async Task<bool> Handle(CheckPermissionQuery request,CancellationToken cancellationToken)
    {
        return await permissions.HasPermissionAsync(
            request.UserId,
            request.EventId,
            request.Permission,
            cancellationToken);
    }
}
