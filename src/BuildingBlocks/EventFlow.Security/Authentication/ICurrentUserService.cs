namespace EventFlow.Security.Authentication;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    Guid UserId { get; }
}
