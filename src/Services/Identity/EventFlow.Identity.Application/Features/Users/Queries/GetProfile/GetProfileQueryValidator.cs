using FluentValidation;

namespace EventFlow.Identity.Application.Features.Users.Queries.GetProfile;

public sealed class GetProfileQueryValidator : AbstractValidator<GetProfileQuery>
{
    public GetProfileQueryValidator()
    {
    }
}
