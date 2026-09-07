

using FluentValidation;

namespace EventFlow.Identity.Application.Commands.AssignUserRole
{
    public sealed class AssignUserRoleCommandValidator: AbstractValidator<AssignUserRoleCommand>
    {
        public AssignUserRoleCommandValidator()
        {
            // Validate required identifiers.

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
}
