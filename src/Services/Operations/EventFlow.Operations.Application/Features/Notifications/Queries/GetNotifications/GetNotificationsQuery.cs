using EventFlow.Operations.Application.Contracts;
using MediatR;

namespace EventFlow.Operations.Application.Features.Notifications.Queries.GetNotifications;

public sealed record GetNotificationsQuery(
    Guid EventId) : IRequest<IReadOnlyList<NotificationDto>>;
