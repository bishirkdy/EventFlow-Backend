namespace EventFlow.Registration.Application.Contracts.Certificates;

public sealed class CertificateGenerationResultDto
{
    public int Generated { get; set; }
    public int Skipped { get; set; }
    public int Failed { get; set; }
    public List<CertificateDto> Certificates { get; set; } = [];
}
