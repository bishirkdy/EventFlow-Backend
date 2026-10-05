namespace EventFlow.Registration.Application.Contracts.Certificates;

public sealed class CertificateDownloadDto
{
    public string CertificateNumber { get; set; } = "";
    public string FileName { get; set; } = "";
    public string ContentType { get; set; } = "application/pdf";
    public byte[] Content { get; set; } = [];
}
