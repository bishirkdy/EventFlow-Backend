using FluentValidation;

namespace EventFlow.Identity.Application.Features.Queries.CheckPermission;

public sealed class CheckPermissionQueryValidator : AbstractValidator<CheckPermissionQuery>
{
    public CheckPermissionQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Permission)
            .NotEmpty()
            .WithMessage("Permission is required.")
            .MaximumLength(256)
            .WithMessage("Permission cannot exceed 256 characters.");
    }
}
