namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationAnalytics;

public sealed record RegistrationTrendPointResponse(
    string Date,
    int Total,
    int Approved,
    int Pending);
