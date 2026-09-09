using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Event.Application.Features.Sections.Commands.CreateSection
{
    public sealed class CreateSectionCommandHandler(ISectionRepository sectionRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<CreateSectionCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreateSectionCommand request,
            CancellationToken cancellationToken)
        {
            // Create section
            var section = new Section(
                request.EventId,
                request.Name,
                request.Description,
                request.DisplayOrder);

            await sectionRepository.AddAsync(section, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return section.Id;
        }
    }
}
