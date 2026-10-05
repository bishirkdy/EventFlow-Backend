using FluentValidation;

namespace EventFlow.Identity.Application.Features.Commands.RemoveUserRole;

public sealed class RemoveUserRoleCommandValidator : AbstractValidator<RemoveUserRoleCommand>
{
    public RemoveUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required.");
    }
}
