using FluentValidation;

namespace EventFlow.Event.Application.Features.Queries.GetEventPhotos
{
    public sealed class GetEventPhotosQueryValidator : AbstractValidator<GetEventPhotosQuery>
    {
        public GetEventPhotosQueryValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0.");
        }
    }
}
