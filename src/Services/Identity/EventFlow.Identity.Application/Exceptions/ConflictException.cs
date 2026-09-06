namespace EventFlow.Identity.Application.Exceptions
{
    // Represents a business/application conflict.
    public sealed class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
