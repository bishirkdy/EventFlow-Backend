namespace EventFlow.Registration.Application.Contracts.Certificates;

public sealed class CertificateEligibilityDto
{
    public int TotalRegistrations { get; set; }
    public int EligibleCount { get; set; }
    public int AlreadyIssuedCount { get; set; }
    public int IneligibleCount { get; set; }
    public List<CertificateEligibilityItemDto> Items { get; set; } = [];
}
