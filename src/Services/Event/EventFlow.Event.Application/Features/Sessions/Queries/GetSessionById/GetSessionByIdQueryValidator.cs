

using FluentValidation;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionById
{
    public sealed class GetSessionByIdQueryValidator : AbstractValidator<GetSessionByIdQuery>
    {
        public GetSessionByIdQueryValidator()
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
