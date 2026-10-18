using EventFlow.Contracts.Common;
using EventFlow.Event.Domain.Enums;
using MediatR;

namespace EventFlow.Event.Application.Features.Admin.Queries.GetAdminEvents;

public sealed record GetAdminEventsQuery(
    int Page = 1,
    int PageSize = 20,
    EventStatus? Status = null,
    string? Search = null
) : IRequest<PaginatedResponse<AdminEventListItemResponse>>;
