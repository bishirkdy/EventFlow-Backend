using FluentValidation;


namespace EventFlow.Event.Application.Features.Sections.Queries.GetSectionById
{
    public sealed class GetSectionByIdQueryValidator: AbstractValidator<GetSectionByIdQuery>
    {
        public GetSectionByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Section ID is required.");
        }
    }
}
