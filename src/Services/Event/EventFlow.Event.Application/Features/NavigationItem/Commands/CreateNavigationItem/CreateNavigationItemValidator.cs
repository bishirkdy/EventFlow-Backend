using FluentValidation;
namespace EventFlow.Event.Application.Features.NavigationItem.Commands.CreateNavigationItem
{
    public sealed class CreateNavigationItemValidator: AbstractValidator<CreateNavigationItemCommand>
    {
        public CreateNavigationItemValidator()
        {
            RuleFor(x => x.NavigationMenuId)
                .NotEmpty()
                .WithMessage("Navigation menu ID is required.");

            RuleFor(x => x.Label)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Navigation item label is required.");

            RuleFor(x => x.Url)
                .MaximumLength(1000);

            RuleFor(x => x)
                .Must(x => (string.IsNullOrWhiteSpace(x.Url) && x.PageId.HasValue) ||
                           (!string.IsNullOrWhiteSpace(x.Url) && !x.PageId.HasValue))
                .WithMessage("Provide either a page or a custom URL, but not both.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Display order must be zero or greater.");
        }
    }
}
