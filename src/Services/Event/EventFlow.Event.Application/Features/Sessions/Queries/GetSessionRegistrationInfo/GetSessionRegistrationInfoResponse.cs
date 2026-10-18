namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionRegistrationInfo;

public sealed record GetSessionRegistrationInfoResponse(
    bool Exists,
    Guid EventId,
    Guid SessionId,
    Guid SectionId,
    bool AllowRegistration,
    int? Capacity,
    string Status);
