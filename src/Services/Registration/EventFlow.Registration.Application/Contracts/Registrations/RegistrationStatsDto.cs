namespace EventFlow.Registration.Application.Contracts.Registrations;

public sealed class RegistrationStatsDto
{
    public int Total { get; set; }
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Cancelled { get; set; }
    public int Waitlisted { get; set; }
    public int Participants { get; set; }
    public int ActiveTickets { get; set; }
}
