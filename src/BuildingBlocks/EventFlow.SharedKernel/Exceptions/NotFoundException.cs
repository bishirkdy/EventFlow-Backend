

namespace EventFlow.SharedKernel.Exceptions
{
    public sealed class NotFoundException : EventFlowException
    {
        public NotFoundException(string message) : base(message) { }
    }
}
