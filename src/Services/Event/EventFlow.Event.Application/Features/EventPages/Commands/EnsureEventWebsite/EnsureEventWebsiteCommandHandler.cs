using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Services;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.EnsureEventWebsite;

public sealed class EnsureEventWebsiteCommandHandler(
    IEventRepository eventRepository,
    IEventWebsiteProvisioningService provisioning)
    : IRequestHandler<EnsureEventWebsiteCommand>
{
    public async Task Handle(EnsureEventWebsiteCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await eventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (eventEntity is null)
        {
            throw new NotFoundException("Event not found.");
        }

        await provisioning.EnsureInitialWebsiteAsync(request.EventId, cancellationToken);
    }
}
