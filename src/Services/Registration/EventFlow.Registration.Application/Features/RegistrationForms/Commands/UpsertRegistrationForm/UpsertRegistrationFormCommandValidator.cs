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
    }
}
