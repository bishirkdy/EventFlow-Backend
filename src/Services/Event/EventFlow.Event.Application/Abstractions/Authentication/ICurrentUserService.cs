

namespace EventFlow.Event.Application.Abstractions.Authentication
{
    //For getting current user
    public interface ICurrentUserService
    {
        Guid UserId { get; }
    }
}
