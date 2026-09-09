using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Event.Application.Features.Sections.Commands.DeleteSection
{
    public sealed class DeleteSectionCommandValidator: AbstractValidator<DeleteSectionCommand>
    {
        public DeleteSectionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Section ID is required.");

            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
