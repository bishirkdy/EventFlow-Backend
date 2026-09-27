using FluentValidation;

namespace EventFlow.Event.Application.Features.EventPages.Commands.ReorderEventPages
{
    public sealed class ReorderEventPagesValidator : AbstractValidator<ReorderEventPagesCommand>
    {
        public ReorderEventPagesValidator()
        {
            RuleFor(x => x.EventId).NotEmpty();
            RuleFor(x => x.PageIds).NotNull();
        }
    }
}
