namespace EventFlow.Identity.Application.Exceptions
{
    // Represents an authentication failure.
    public sealed class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message): base(message){}
    }
}
