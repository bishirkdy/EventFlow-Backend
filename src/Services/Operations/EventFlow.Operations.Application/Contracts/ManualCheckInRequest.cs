namespace EventFlow.Operations.Application.Contracts;

public sealed record ManualCheckInRequest(
    Guid RegistrationId,
    Guid? SectionId,
    Guid? SessionId);
