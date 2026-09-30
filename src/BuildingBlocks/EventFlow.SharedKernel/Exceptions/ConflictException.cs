

namespace EventFlow.SharedKernel.Exceptions
{
    public sealed class ConflictException : EventFlowException
    {
        public ConflictException(string message): base(message){}
    }
}
