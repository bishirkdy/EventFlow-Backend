namespace EventFlow.Registration.Application.Contracts.Registrations;

public sealed class CreateRegistrationRequest
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string? Organization { get; set; }
    public string? Designation { get; set; }
    public Dictionary<Guid, string> Answers { get; set; } = new();
}

public sealed class UpdateRegistrationRequest
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string? Organization { get; set; }
    public string? Designation { get; set; }
    public Dictionary<Guid, string> Answers { get; set; } = new();
}

public sealed class RejectRegistrationRequest
{
    public string Reason { get; set; } = "";
}

public sealed class CancelRegistrationRequest
{
    public string? Reason { get; set; }
}