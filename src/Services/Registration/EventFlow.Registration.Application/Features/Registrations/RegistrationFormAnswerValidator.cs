using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Application.Features.Registrations;

internal static class RegistrationFormAnswerValidator
{
    public static IReadOnlyList<string> Validate(
        RegistrationForm? form,
        IReadOnlyDictionary<Guid, string> answers)
    {
        if (form is null)
        {
            return [];
        }

        var errors = new List<string>();
        var fields = form.Fields.ToDictionary(x => x.Id);

        foreach (var answerId in answers.Keys)
        {
            if (!fields.ContainsKey(answerId))
            {
                errors.Add("One or more submitted answers do not belong to this registration form.");
                break;
            }
        }

        foreach (var field in form.Fields.OrderBy(x => x.DisplayOrder))
        {
            answers.TryGetValue(field.Id, out var rawValue);
            var value = rawValue?.Trim() ?? string.Empty;

            if (field.IsRequired && string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"Required field '{field.Label}' is missing.");
                continue;
            }

            if (field.IsRequired && field.FieldType == RegistrationFieldType.Checkbox)
            {
                if (!bool.TryParse(value, out var checkedValue) || !checkedValue)
                {
                    errors.Add($"Required field '{field.Label}' must be checked.");
                    continue;
                }
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            ValidateField(field, value, errors);
        }

        return errors;
    }

    private static void ValidateField(
        RegistrationFormField field,
        string value,
        ICollection<string> errors)
    {
        switch (field.FieldType)
        {
            case RegistrationFieldType.Email:
                if (!new EmailAddressAttribute().IsValid(value))
                {
                    errors.Add($"Field '{field.Label}' must contain a valid email address.");
                }
                break;

            case RegistrationFieldType.Number:
                if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
                {
                    errors.Add($"Field '{field.Label}' must contain a valid number.");
                }
                break;

            case RegistrationFieldType.Date:
                if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    errors.Add($"Field '{field.Label}' must contain a valid date.");
                }
                break;

            case RegistrationFieldType.Select:
            case RegistrationFieldType.Radio:
                if (!GetOptions(field.OptionsJson).Contains(value, StringComparer.Ordinal))
                {
                    errors.Add($"Field '{field.Label}' contains an invalid option.");
                }
                break;

            case RegistrationFieldType.Checkbox:
                if (!bool.TryParse(value, out _))
                {
                    errors.Add($"Field '{field.Label}' must be true or false.");
                }
                break;
        }

        ApplyValidationRules(field, value, errors);
    }

    private static void ApplyValidationRules(
        RegistrationFormField field,
        string value,
        ICollection<string> errors)
    {
        if (string.IsNullOrWhiteSpace(field.ValidationJson))
        {
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(field.ValidationJson);
            var root = document.RootElement;

            if (root.TryGetProperty("minLength", out var minLengthElement) &&
                minLengthElement.TryGetInt32(out var minLength) &&
                value.Length < minLength)
            {
                errors.Add($"Field '{field.Label}' must contain at least {minLength} characters.");
            }

            if (root.TryGetProperty("maxLength", out var maxLengthElement) &&
                maxLengthElement.TryGetInt32(out var maxLength) &&
                value.Length > maxLength)
            {
                errors.Add($"Field '{field.Label}' must contain at most {maxLength} characters.");
            }

            if (root.TryGetProperty("min", out var minElement) &&
                minElement.TryGetDecimal(out var min) &&
                decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number) &&
                number < min)
            {
                errors.Add($"Field '{field.Label}' must be at least {min}.");
            }

            if (root.TryGetProperty("max", out var maxElement) &&
                maxElement.TryGetDecimal(out var max) &&
                decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var maxNumber) &&
                maxNumber > max)
            {
                errors.Add($"Field '{field.Label}' must be at most {max}.");
            }

            if (root.TryGetProperty("pattern", out var patternElement) &&
                patternElement.ValueKind == JsonValueKind.String)
            {
                var pattern = patternElement.GetString();

                if (!string.IsNullOrWhiteSpace(pattern))
                {
                    try
                    {
                        if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)))
                        {
                            errors.Add($"Field '{field.Label}' has an invalid format.");
                        }
                    }
                    catch (ArgumentException)
                    {
                        errors.Add($"Field '{field.Label}' has an invalid validation pattern.");
                    }
                    catch (RegexMatchTimeoutException)
                    {
                        errors.Add($"Field '{field.Label}' has an invalid validation pattern.");
                    }
                }
            }
        }
        catch (JsonException)
        {
            errors.Add($"Validation rules for '{field.Label}' contain invalid JSON.");
        }
    }

    private static IReadOnlyList<string> GetOptions(string? optionsJson)
    {
        if (string.IsNullOrWhiteSpace(optionsJson))
        {
            return [];
        }

        try
        {
            using var document = JsonDocument.Parse(optionsJson);

            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            return document.RootElement
                .EnumerateArray()
                .Where(x => x.ValueKind == JsonValueKind.String)
                .Select(x => x.GetString() ?? string.Empty)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }
        catch (JsonException)
        {
            return [];
        }
    }
}
