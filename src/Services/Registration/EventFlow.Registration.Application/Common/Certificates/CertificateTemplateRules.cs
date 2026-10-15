namespace EventFlow.Registration.Application.Common.Certificates;

public static class CertificateTemplateRules
{
    public const int MaxFields = 30;

    public const double MinFontSizePt = 6;
    public const double MaxFontSizePt = 96;

    public static readonly IReadOnlySet<string> AllowedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "participantName",
        "eventTitle",
        "eventDates",
        "certificateNumber",
        "issueDate",
        "signatoryName",
        "signatoryTitle",
        "title",
        "subtitle",
        "verificationQr",
        "customText"
    };

    public static readonly IReadOnlySet<string> AllowedFontFamilies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        // Bundled with QuestPDF - always available, including in containers.
        "Lato",
        "Arial",
        "Times New Roman",
        "Courier New",
        "Verdana",
        "Georgia",
        "Calibri"
    };

    public static bool IsAllowedKey(string key) =>
        !string.IsNullOrWhiteSpace(key) && AllowedKeys.Contains(key.Trim());

    public static bool IsHexColor(string? color)
    {
        if (string.IsNullOrWhiteSpace(color))
        {
            return false;
        }

        var value = color.Trim();

        if (value.Length == 7 && value[0] == '#')
        {
            return value.Skip(1).All(Uri.IsHexDigit);
        }

        if (value.Length == 6)
        {
            return value.All(Uri.IsHexDigit);
        }

        return false;
    }
}
