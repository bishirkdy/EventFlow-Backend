using EventFlow.Identity.Domain.Enums;
using EventFlow.SharedKernel.Validation;
using FluentValidation;

namespace EventFlow.Identity.Application.Features.EventInvitations.Commands.CreateEventInvitation;

public sealed class CreateEventInvitationCommandValidator
    : AbstractValidator<CreateEventInvitationCommand>
{
    public CreateEventInvitationCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .MaximumLength(320)
            .WithMessage("Email cannot exceed 320 characters.")
            .Must(ValidationRules.IsValidEmail)
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid invitation type.");

        RuleFor(x => x.ScopeId)
            .NotNull()
            .WithMessage("A session must be selected for this scope.")
            .When(x => x.Type == EventInvitationType.AttendanceStaff
                       && x.ScopeType.HasValue
                       && x.ScopeType.Value != InvitationScopeType.Event);
    }
}
