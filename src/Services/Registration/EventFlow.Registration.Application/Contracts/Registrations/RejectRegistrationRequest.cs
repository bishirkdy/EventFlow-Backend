namespace EventFlow.Registration.Application.Contracts.Registrations;

public sealed class RejectRegistrationRequest
{
    public string Reason { get; set; } = "";
}
