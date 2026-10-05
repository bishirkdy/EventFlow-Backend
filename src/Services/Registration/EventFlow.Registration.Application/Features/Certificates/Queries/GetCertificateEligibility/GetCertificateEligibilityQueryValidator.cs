using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateEligibility;

public sealed class GetCertificateEligibilityQueryValidator : AbstractValidator<GetCertificateEligibilityQuery>
{
    public GetCertificateEligibilityQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
