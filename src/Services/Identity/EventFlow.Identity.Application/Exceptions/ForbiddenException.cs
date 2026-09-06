namespace EventFlow.Identity.Application.Exceptions
{
    // Represents an authorization failure.
    public sealed class ForbiddenException : Exception
    {
        public ForbiddenException(string message): base(message){}
    }
}