using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetMyCertificates;

public sealed class GetMyCertificatesQueryValidator : AbstractValidator<GetMyCertificatesQuery>
{
    public GetMyCertificatesQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
