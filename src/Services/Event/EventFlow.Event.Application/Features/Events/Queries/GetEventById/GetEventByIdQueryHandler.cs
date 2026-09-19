using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Services;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetEventById;

public sealed class GetEventByIdQueryHandler(
    IEventRepository eventRepository,
    IMapper mapper,
    IUserDirectoryClient userDirectoryClient)
    : IRequestHandler<GetEventByIdQuery, GetEventByIdResponse?>
{
    public async Task<GetEventByIdResponse?> Handle(
        GetEventByIdQuery request,
        CancellationToken cancellationToken)
    {
        var eventEntity = await eventRepository.GetByIdWithImagesAsync(
            request.Id,
            cancellationToken);

        if (eventEntity is null)
            return null;

        var response = mapper.Map<GetEventByIdResponse>(eventEntity);

        response.CreatedByName = await userDirectoryClient.GetDisplayNameAsync(
            eventEntity.CreatedBy,
            cancellationToken);

        return response;
    }
}
