namespace EventFlow.Operations.Application.Abstractions;

public sealed record VerifiedTicket(
    Guid RegistrationId,
    Guid ParticipantId,
    Guid ParticipantUserId,
    bool IsValid,
    string? Message,
    RegistrationScope ScopeType = RegistrationScope.Event,
    Guid? ScopeId = null);
