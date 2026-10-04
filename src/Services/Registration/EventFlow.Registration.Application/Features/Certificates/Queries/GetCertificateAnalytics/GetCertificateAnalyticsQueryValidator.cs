using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateAnalytics;

internal sealed class GetCertificateAnalyticsQueryValidator
    : AbstractValidator<GetCertificateAnalyticsQuery>
{
    public GetCertificateAnalyticsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
