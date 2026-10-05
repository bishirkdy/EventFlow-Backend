using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetRegistrationFeature;

public sealed record GetRegistrationFeatureQuery(Guid EventId) : IRequest<bool>;
