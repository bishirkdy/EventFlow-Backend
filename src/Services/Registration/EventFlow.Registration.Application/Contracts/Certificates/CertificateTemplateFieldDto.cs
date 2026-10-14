namespace EventFlow.Registration.Application.Contracts.Certificates;

public sealed class CertificateTemplateFieldDto
{
    public string Key { get; set; } = "";
    public string? Text { get; set; }
    public double XPercent { get; set; }
    public double YPercent { get; set; }
    public double WidthPercent { get; set; } = 40;
    public double FontSizePt { get; set; } = 18;
    public string FontColor { get; set; } = "#111827";
    public bool Bold { get; set; }
    public string Align { get; set; } = "Center";
    public string? FontFamily { get; set; }
}
