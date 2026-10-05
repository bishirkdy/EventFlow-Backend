

using FluentValidation;

namespace EventFlow.Event.Application.Features.EventPages.Commands.DeleteEventPage
{
    public sealed class DeleteEventPageCommandValidator: AbstractValidator<DeleteEventPageCommand>
    {
        public DeleteEventPageCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Page ID is required.");
        }
    }
}
