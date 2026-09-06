namespace EventFlow.Identity.Application.Exceptions
{
    // Represents a resource that could not be found.
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
