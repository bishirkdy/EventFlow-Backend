using FluentValidation;

namespace EventFlow.Operations.Application.Features.Notifications.Commands.QueueNotification;

public sealed class QueueNotificationCommandValidator
    : AbstractValidator<QueueNotificationCommand>
{
    public QueueNotificationCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.RecipientEmail)
            .NotEmpty()
            .WithMessage("Recipient email and subject are required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(320)
            .WithMessage("Recipient email cannot exceed 320 characters.");

        RuleFor(x => x.Subject)
            .NotEmpty()
            .WithMessage("Recipient email and subject are required.");
    }
}
