

namespace EventFlow.SharedKernel.Exceptions
{
    public sealed class ForbiddenException : EventFlowException
    {
        public ForbiddenException(string message): base(message){}
    }
}
