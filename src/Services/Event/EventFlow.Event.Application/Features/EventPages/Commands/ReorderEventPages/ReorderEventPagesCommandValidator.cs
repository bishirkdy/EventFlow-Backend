using FluentValidation;

namespace EventFlow.Event.Application.Features.EventPages.Commands.ReorderEventPages
{
    public sealed class ReorderEventPagesCommandValidator : AbstractValidator<ReorderEventPagesCommand>
    {
        public ReorderEventPagesCommandValidator()
        {
            RuleFor(x => x.EventId).NotEmpty();
            RuleFor(x => x.PageIds).NotNull();
        }
    }
}
