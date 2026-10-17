namespace EventFlow.Registration.Application.Contracts.Events;

public sealed record SessionRegistrationInfo(
    bool Exists,
    Guid EventId,
    Guid SessionId,
    Guid SectionId,
    bool AllowRegistration,
    int? Capacity,
    string Status);
