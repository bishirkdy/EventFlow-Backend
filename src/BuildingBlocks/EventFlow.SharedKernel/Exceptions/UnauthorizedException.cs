

namespace EventFlow.SharedKernel.Exceptions
{
    public sealed class UnauthorizedException : EventFlowException
    {
        public UnauthorizedException(string message): base(message){}
    }
}
