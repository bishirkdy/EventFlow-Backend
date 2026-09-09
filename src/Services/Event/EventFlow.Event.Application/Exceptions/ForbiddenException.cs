
namespace EventFlow.Event.Application.Exceptions
{
    // Represents a forbidden
    public sealed class ForbiddenExeption : Exception
    {
        public ForbiddenExeption(string message): base(message)
        {
        }
    }
}
