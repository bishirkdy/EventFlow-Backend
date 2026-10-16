namespace EventFlow.Operations.Infrastructure.Background;

public enum MessageOutcome
{
    Processed = 0,
    Duplicate = 1,
    DeadLettered = 2
}
