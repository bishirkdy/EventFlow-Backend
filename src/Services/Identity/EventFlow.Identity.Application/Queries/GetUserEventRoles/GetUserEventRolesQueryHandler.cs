

using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.UserEventRoles;
using MediatR;

namespace EventFlow.Identity.Application.Queries.GetUserEventRoles
{
    public class GetUserEventRolesQueryHandler
        : IRequestHandler<
            GetUserEventRolesQuery,
            List<UserEventRoleResponse>>
    {
        private readonly IUserEventRoleRepository _repository;

        public GetUserEventRolesQueryHandler(
            IUserEventRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UserEventRoleResponse>> Handle(
            GetUserEventRolesQuery request,
            CancellationToken cancellationToken)
        {
            // Purpose: Get all roles assigned to the user for this event.
            var userEventRoles =
                await _repository.GetByUserAndEventAsync(
                    request.UserId,
                    request.EventId,
                    cancellationToken);

            // Purpose: Return only the data needed by the API.
            return userEventRoles
                .Select(x => new UserEventRoleResponse(
                    x.Id,
                    x.RoleId,
                    x.Role.Name,
                    x.EventId))
                .ToList();
        }
    }
}
