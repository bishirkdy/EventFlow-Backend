using System.Text.RegularExpressions;

namespace EventFlow.SharedKernel.Validation;

public static class ValidationRules
{
    private static readonly Regex EmailRegex = new(
        "^[A-Za-z][A-Za-z0-9._-]*@[A-Za-z0-9-]+\\.[A-Za-z]{2,}$",
        RegexOptions.Compiled);

    private static readonly Regex PhoneRegex = new(
        "^\\+?[0-9\\s().-]+$",
        RegexOptions.Compiled);

    public static bool IsValidEmail(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) && EmailRegex.IsMatch(value.Trim());
    }

    public static bool IsValidOptionalPhone(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        var trimmed = value.Trim();

        if (!PhoneRegex.IsMatch(trimmed))
        {
            return false;
        }

        var digits = trimmed.Count(char.IsDigit);

        return digits is >= 7 and <= 15;
    }
}
