using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Services;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetMyEvents;

public sealed class GetMyEventsQueryHandler(
    IEventRepository eventRepository,
    IMapper mapper,
    IUserDirectoryClient userDirectoryClient)
    : IRequestHandler<GetMyEventsQuery, IReadOnlyList<GetMyEventsResponse>>
{
    public async Task<IReadOnlyList<GetMyEventsResponse>> Handle(
        GetMyEventsQuery request,
        CancellationToken cancellationToken)
    {
        var events = await eventRepository.GetByCreatedByAsync(
            request.UserId,
            cancellationToken);

        var responses = mapper.Map<List<GetMyEventsResponse>>(events);

        var displayName = await userDirectoryClient.GetDisplayNameAsync(
            request.UserId,
            cancellationToken);

        foreach (var response in responses)
        {
            response.CreatedByName = displayName;
        }

        return responses;
    }
}