using FluentValidation;

namespace EventFlow.Event.Application.Features.Feedback.Queries.GetFeedbackResults;

public sealed class GetFeedbackResultsQueryValidator : AbstractValidator<GetFeedbackResultsQuery>
{
    public GetFeedbackResultsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.RequestedBy)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
