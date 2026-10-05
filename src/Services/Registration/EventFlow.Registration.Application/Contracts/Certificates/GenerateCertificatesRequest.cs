namespace EventFlow.Registration.Application.Contracts.Certificates;

public sealed class GenerateCertificatesRequest
{
    public List<Guid>? RegistrationIds { get; set; }
}
