
namespace EventFlow.Event.Application.Exceptions
{
    // Represents a requested resource that does not exist.
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string message): base(message)
        {
        }
    }
}
