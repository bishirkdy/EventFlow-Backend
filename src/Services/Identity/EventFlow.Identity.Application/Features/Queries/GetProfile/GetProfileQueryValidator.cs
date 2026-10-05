using FluentValidation;

namespace EventFlow.Identity.Application.Features.Queries.GetProfile;

public sealed class GetProfileQueryValidator : AbstractValidator<GetProfileQuery>
{
    public GetProfileQueryValidator()
    {
    }
}
