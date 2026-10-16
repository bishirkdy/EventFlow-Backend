using EventFlow.Messaging.Events;

namespace EventFlow.Messaging.Tests;

public sealed record TestEvent : IntegrationEvent
{
    public string Name { get; init; } = string.Empty;
}
