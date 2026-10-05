using FluentValidation;

namespace EventFlow.Event.Application.Features.EventType.Queries.GetActiveEventTypes;

public sealed class GetActiveEventTypesQueryValidator
    : AbstractValidator<GetActiveEventTypesQuery>
{
    public GetActiveEventTypesQueryValidator()
    {
    }
}
