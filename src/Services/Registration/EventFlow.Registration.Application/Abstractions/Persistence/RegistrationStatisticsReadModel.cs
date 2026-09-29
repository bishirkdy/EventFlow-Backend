namespace EventFlow.Registration.Application.Abstractions.Persistence;

public sealed record RegistrationStatisticsReadModel(
    int Total,
    int Pending,
    int Approved,
    int Rejected,
    int Cancelled,
    int Waitlisted,
    int Participants,
    int ActiveTickets);
