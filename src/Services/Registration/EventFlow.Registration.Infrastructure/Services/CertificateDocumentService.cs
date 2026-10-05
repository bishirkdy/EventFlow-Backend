using EventFlow.Registration.Application.Abstractions.Services;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EventFlow.Registration.Infrastructure.Services;

public sealed class CertificateDocumentService : ICertificateDocumentService
{
    static CertificateDocumentService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        // QuestPDF 2026.9+ no longer resolves system font families implicitly;
        // the certificate layout uses Arial, which comes from the OS fonts.
        QuestPDF.Settings.UseSystemFonts = true;
    }

    public byte[] Generate(CertificateDocumentData data)
    {
        var themeColor = NormalizeColor(data.ThemeColor);
        var qrPng = GenerateQrPng(data.VerificationUrl);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(28);
                page.DefaultTextStyle(x =>
                    x.FontFamily("Arial").FontSize(11).FontColor(Colors.Grey.Darken3));

                page.Content()
                    .Border(3)
                    .BorderColor(themeColor)
                    .Padding(30)
                    .Column(column =>
                    {
                        column.Spacing(6);

                        column.Item()
                            .AlignCenter()
                            .Text(data.Title)
                            .FontSize(30)
                            .Bold()
                            .FontColor(themeColor);

                        if (!string.IsNullOrWhiteSpace(data.Subtitle))
                        {
                            column.Item()
                                .AlignCenter()
                                .Text(data.Subtitle)
                                .FontSize(13)
                                .FontColor(Colors.Grey.Medium);
                        }

                        column.Item()
                            .PaddingTop(18)
                            .AlignCenter()
                            .Text("This is to certify that")
                            .FontSize(13)
                            .FontColor(Colors.Grey.Darken1);

                        column.Item()
                            .AlignCenter()
                            .Text(data.ParticipantName)
                            .FontSize(34)
                            .Bold()
                            .FontColor(Colors.Black);

                        column.Item()
                            .AlignCenter()
                            .Text("has participated in")
                            .FontSize(13)
                            .FontColor(Colors.Grey.Darken1);

                        column.Item()
                            .AlignCenter()
                            .Text(data.EventName)
                            .FontSize(22)
                            .SemiBold()
                            .FontColor(themeColor);

                        if (!string.IsNullOrWhiteSpace(data.EventDatesText))
                        {
                            column.Item()
                                .AlignCenter()
                                .Text(data.EventDatesText)
                                .FontSize(12)
                                .FontColor(Colors.Grey.Darken1);
                        }

                        column.Item()
                            .PaddingTop(20)
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Column(signature =>
                                    {
                                        signature.Item()
                                            .AlignCenter()
                                            .Width(170)
                                            .LineHorizontal(1)
                                            .LineColor(Colors.Grey.Medium);

                                        signature.Item()
                                            .AlignCenter()
                                            .PaddingTop(4)
                                            .Text(data.SignatoryName ?? "")
                                            .FontSize(12)
                                            .SemiBold();

                                        signature.Item()
                                            .AlignCenter()
                                            .Text(data.SignatoryTitle ?? "")
                                            .FontSize(10)
                                            .FontColor(Colors.Grey.Darken1);
                                    });

                                row.ConstantItem(100)
                                    .AlignCenter()
                                    .Column(qr =>
                                    {
                                        qr.Item().AlignCenter().Width(76).Image(qrPng);
                                        qr.Item()
                                            .AlignCenter()
                                            .PaddingTop(4)
                                            .Text("Scan to verify")
                                            .FontSize(8)
                                            .FontColor(Colors.Grey.Darken1);
                                    });

                                row.RelativeItem()
                                    .Column(footer =>
                                    {
                                        footer.Item()
                                            .AlignCenter()
                                            .Width(170)
                                            .LineHorizontal(1)
                                            .LineColor(Colors.Grey.Medium);

                                        footer.Item()
                                            .AlignCenter()
                                            .PaddingTop(4)
                                            .Text(data.IssuedAtUtc.ToLocalTime().ToString("d"))
                                            .FontSize(12)
                                            .SemiBold();

                                        footer.Item()
                                            .AlignCenter()
                                            .Text("Date of issue")
                                            .FontSize(10)
                                            .FontColor(Colors.Grey.Darken1);
                                    });
                            });

                        column.Item()
                            .PaddingTop(14)
                            .AlignCenter()
                            .Text(text =>
                            {
                                text.Span("Certificate No: ").FontSize(9).FontColor(Colors.Grey.Medium);
                                text.Span(data.CertificateNumber).FontSize(9).SemiBold();
                            });
                    });
            });
        });

        return document.GeneratePdf();
    }

    private static byte[] GenerateQrPng(string payload)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(
            payload,
            QRCodeGenerator.ECCLevel.Q);
        var png = new PngByteQRCode(data);
        return png.GetGraphic(8);
    }

    private static string NormalizeColor(string color)
    {
        var value = string.IsNullOrWhiteSpace(color) ? "#2563EB" : color.Trim();

        if (value.StartsWith('#') && value.Length == 7)
        {
            return value;
        }

        if (value.Length == 6)
        {
            return $"#{value}";
        }

        return "#2563EB";
    }
}
