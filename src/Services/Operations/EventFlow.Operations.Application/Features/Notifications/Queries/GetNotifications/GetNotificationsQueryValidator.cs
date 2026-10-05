using FluentValidation;

namespace EventFlow.Operations.Application.Features.Notifications.Queries.GetNotifications;

public sealed class GetNotificationsQueryValidator
    : AbstractValidator<GetNotificationsQuery>
{
    public GetNotificationsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
