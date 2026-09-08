using MediatR;


namespace EventFlow.Event.Application.Features.EventSettings.Commands.ResetEventSettings
{
    // Command
    public sealed record ResetEventSettingsCommand(Guid EventId) : IRequest;
}
