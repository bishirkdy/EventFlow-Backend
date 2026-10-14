using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EventFlow.Registration.Infrastructure.Services;

public sealed class CertificateDocumentService : ICertificateDocumentService
{
    private const string DefaultTemplateFont = "Arial";
    private const string DefaultFieldColor = "#111827";

    static CertificateDocumentService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        // QuestPDF 2026.9+ no longer resolves system font families implicitly;
        // the certificate layout uses Arial, which comes from the OS fonts.
        QuestPDF.Settings.UseSystemFonts = true;
    }

    public byte[] Generate(CertificateDocumentData data)
    {
        if (data.UseImageTemplate &&
            data.BackgroundImage is { Length: > 0 } &&
            data.TemplateFields is { Count: > 0 } &&
            data.BackgroundImageWidth > 0 &&
            data.BackgroundImageHeight > 0)
        {
            return GenerateImageTemplate(data);
        }

        return GenerateClassic(data);
    }

    private static byte[] GenerateImageTemplate(CertificateDocumentData data)
    {
        var pageHeight = PageSizes.A4.Landscape().Height;
        var pageWidth = pageHeight * data.BackgroundImageWidth / data.BackgroundImageHeight;
        var qrPng = GenerateQrPng(data.VerificationUrl);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(pageWidth, pageHeight);
                page.Margin(0);
                page.DefaultTextStyle(x =>
                    x.FontFamily(DefaultTemplateFont).FontSize(12).FontColor(Colors.Grey.Darken3));

                page.Content()
                    .Layers(layers =>
                    {
                        layers.Layer()
                            .Image(data.BackgroundImage!)
                            .FitUnproportionally();

                        layers.PrimaryLayer()
                            .Width(pageWidth)
                            .Height(pageHeight)
                            .Column(column =>
                            {
                                foreach (var field in data.TemplateFields!)
                                {
                                    RenderTemplateField(column, field, data, pageWidth, pageHeight, qrPng);
                                }
                            });
                    });
            });
        });

        return document.GeneratePdf();
    }

    private static void RenderTemplateField(
        ColumnDescriptor column,
        CertificateTemplateField field,
        CertificateDocumentData data,
        float pageWidth,
        float pageHeight,
        byte[] qrPng)
    {
        var x = (float)(field.XPercent / 100d * pageWidth);
        var y = (float)(field.YPercent / 100d * pageHeight);
        var boxWidth = Math.Max(1f, (float)(field.WidthPercent / 100d * pageWidth));

        if (field.Key.Equals("verificationQr", StringComparison.OrdinalIgnoreCase))
        {
            column.Item()
                .Unconstrained()
                .OffsetX(x)
                .OffsetY(y)
                .Width(boxWidth)
                .Height(boxWidth)
                .Image(qrPng)
                .FitArea();

            return;
        }

        var value = ResolveFieldValue(field, data);

        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        var fontFamily = string.IsNullOrWhiteSpace(field.FontFamily)
            ? DefaultTemplateFont
            : field.FontFamily!.Trim();

        var descriptor = column.Item()
            .Unconstrained()
            .OffsetX(x)
            .OffsetY(y)
            .Width(boxWidth)
            .Text(value)
            .FontFamily(fontFamily)
            .FontSize((float)field.FontSizePt)
            .FontColor(NormalizeColor(field.FontColor, DefaultFieldColor));

        if (field.Bold)
        {
            descriptor.Bold();
        }

        switch (field.Align)
        {
            case CertificateFieldAlign.Left:
                descriptor.AlignLeft();
                break;
            case CertificateFieldAlign.Right:
                descriptor.AlignRight();
                break;
            default:
                descriptor.AlignCenter();
                break;
        }
    }

    private static string ResolveFieldValue(CertificateTemplateField field, CertificateDocumentData data)
    {
        return field.Key.ToLowerInvariant() switch
        {
            "participantname" => data.ParticipantName,
            "eventtitle" => data.EventName,
            "eventdates" => data.EventDatesText,
            "certificatenumber" => data.CertificateNumber,
            "issuedate" => data.IssuedAtUtc.ToLocalTime().ToString("d"),
            "signatoryname" => data.SignatoryName ?? "",
            "signatorytitle" => data.SignatoryTitle ?? "",
            "title" => data.Title,
            "subtitle" => data.Subtitle,
            "customtext" => field.Text ?? "",
            _ => ""
        };
    }

    private static byte[] GenerateClassic(CertificateDocumentData data)
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

    private static string NormalizeColor(string color, string fallback = "#2563EB")
    {
        var value = string.IsNullOrWhiteSpace(color) ? fallback : color.Trim();

        if (value.StartsWith('#') && value.Length == 7)
        {
            return value;
        }

        if (value.Length == 6)
        {
            return $"#{value}";
        }

        return fallback;
    }
}
