using EventFlow.Messaging.Events;

namespace EventFlow.Operations.Infrastructure.Tests;

public sealed record TestIntegrationEvent : IntegrationEvent
{
    public string Payload { get; init; } = string.Empty;
}
