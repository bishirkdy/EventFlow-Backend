using FluentValidation;

namespace EventFlow.Identity.Application.Features.Commands.AssignOwnerRole;

public sealed class AssignOwnerRoleCommandValidator : AbstractValidator<AssignOwnerRoleCommand>
{
    public AssignOwnerRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
