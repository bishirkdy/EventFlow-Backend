using FluentValidation;
using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Application.Features.RegistrationForms.Commands.UpsertRegistrationForm;

public sealed class UpsertRegistrationFormCommandValidator
    : AbstractValidator<UpsertRegistrationFormCommand>
{
    public UpsertRegistrationFormCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty();

        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Request.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Request.Capacity)
            .GreaterThan(0)
            .When(x => x.Request.Capacity.HasValue);

        RuleFor(x => x.Request)
            .Must(x => x.CapacityMode != CapacityMode.Limited ||
                       (x.Capacity.HasValue && x.Capacity.Value >= 1))
            .WithMessage("A positive capacity is required when capacity mode is limited.");

        RuleFor(x => x.Request)
            .Must(x => x.CapacityMode == CapacityMode.Limited || !x.EnableWaitlist)
            .WithMessage("Waitlist can only be enabled when capacity is limited.");

        RuleFor(x => x.Request)
            .Must(x => !x.OpensAtUtc.HasValue || !x.ClosesAtUtc.HasValue || x.OpensAtUtc < x.ClosesAtUtc)
            .WithMessage("Registration opening time must be before the closing time.");

        RuleForEach(x => x.Request.Fields)
            .ChildRules(field =>
            {
                field.RuleFor(x => x.FieldKey)
                    .NotEmpty()
                    .MaximumLength(100);

                field.RuleFor(x => x.Label)
                    .NotEmpty()
                    .MaximumLength(250);

                field.RuleFor(x => x.DisplayOrder)
                    .GreaterThanOrEqualTo(0);

                field.RuleFor(x => x.FieldType)
                    .IsInEnum();
            });

        RuleFor(x => x.Request.Fields)
            .Must(fields => fields
                .Select(x => x.FieldKey.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() == fields.Count)
            .WithMessage("Registration field keys must be unique.");

        RuleFor(x => x.Request.Fields)
            .Must(fields => fields.All(x => ValidateFieldJson(x) is null))
            .WithMessage(x => FirstFieldJsonError(x.Request.Fields)!);
    }

    private static string? FirstFieldJsonError(
        IEnumerable<RegistrationFormFieldRequest> fields) =>
        fields.Select(ValidateFieldJson).FirstOrDefault(x => x is not null);

    private static string? ValidateFieldJson(RegistrationFormFieldRequest field)
    {
        var isChoice = field.FieldType is
            RegistrationFieldType.Select or
            RegistrationFieldType.Radio;

        if (isChoice)
        {
            if (string.IsNullOrWhiteSpace(field.OptionsJson))
            {
                return $"Field '{field.Label}' requires options.";
            }

            try
            {
                using var document = System.Text.Json.JsonDocument.Parse(field.OptionsJson);

                if (document.RootElement.ValueKind != System.Text.Json.JsonValueKind.Array)
                {
                    return $"Options for '{field.Label}' must be a JSON array.";
                }

                var options = document.RootElement.EnumerateArray().ToArray();

                if (options.Length == 0 || options.Any(x => x.ValueKind != System.Text.Json.JsonValueKind.String || string.IsNullOrWhiteSpace(x.GetString())))
                {
                    return $"Options for '{field.Label}' must contain at least one non-empty string.";
                }
            }
            catch (System.Text.Json.JsonException)
            {
                return $"Options for '{field.Label}' contain invalid JSON.";
            }
        }
        else if (!string.IsNullOrWhiteSpace(field.OptionsJson))
        {
            return $"Field '{field.Label}' does not support options.";
        }

        if (string.IsNullOrWhiteSpace(field.ValidationJson))
        {
            return null;
        }

        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(field.ValidationJson);

            if (document.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object)
            {
                return $"Validation rules for '{field.Label}' must be a JSON object.";
            }
        }
        catch (System.Text.Json.JsonException)
        {
            return $"Validation rules for '{field.Label}' contain invalid JSON.";
        }

        return null;
    }
}
