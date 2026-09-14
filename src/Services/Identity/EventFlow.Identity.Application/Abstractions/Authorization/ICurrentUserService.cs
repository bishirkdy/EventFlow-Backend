

namespace EventFlow.Identity.Application.Abstractions.Authorization
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
    }
}
