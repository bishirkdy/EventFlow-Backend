using EventFlow.Identity.Domain.Enums;
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
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(320)
            .WithMessage("Email cannot exceed 320 characters.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid invitation type.");

        RuleFor(x => x.ScopeId)
            .NotNull()
            .WithMessage("A section or session must be selected for this scope.")
            .When(x => x.Type == EventInvitationType.AttendanceStaff
                       && x.ScopeType.HasValue
                       && x.ScopeType.Value != InvitationScopeType.Event);
    }
}
