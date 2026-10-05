using FluentValidation;

namespace EventFlow.Identity.Application.Features.Commands.AssignEventRole;

public sealed class AssignEventRoleCommandValidator : AbstractValidator<AssignEventRoleCommand>
{
    public AssignEventRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("Role name is required.")
            .MaximumLength(100)
            .WithMessage("Role name cannot exceed 100 characters.");
    }
}
