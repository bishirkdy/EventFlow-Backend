using FluentValidation;


namespace EventFlow.Event.Application.Features.Sessions.Commands.DeleteSession
{
    public sealed class DeleteSessionCommandValidator: AbstractValidator<DeleteSessionCommand>
    {
        public DeleteSessionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Session ID is required.");

            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
