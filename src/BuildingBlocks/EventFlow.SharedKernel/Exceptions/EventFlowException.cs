

namespace EventFlow.SharedKernel.Exceptions
{
    public abstract class EventFlowException : Exception
    {
        protected EventFlowException(string message) : base(message){}
        protected EventFlowException(string message,Exception innerException) : base(message, innerException){}
    }
}
