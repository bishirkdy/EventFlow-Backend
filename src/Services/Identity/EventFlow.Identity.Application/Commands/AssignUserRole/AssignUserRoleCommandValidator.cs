

using FluentValidation;

namespace EventFlow.Identity.Application.Commands.AssignUserRole
{
    public class AssignUserRoleCommandValidator
        : AbstractValidator<AssignUserRoleCommand>
    {
        public AssignUserRoleCommandValidator()
        {
            // Purpose: Validate required identifiers.

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
