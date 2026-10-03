using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.EnsureEventWebsite;

/// <summary>
/// Makes sure an event has the default website pages, page sections and navigation items.
/// Running it repeatedly is safe: existing pages are never touched or duplicated.
/// </summary>
public sealed record EnsureEventWebsiteCommand(Guid EventId) : IRequest;
