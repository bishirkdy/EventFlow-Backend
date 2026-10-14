using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.PreviewCertificate;

public sealed class PreviewCertificateQueryValidator : AbstractValidator<PreviewCertificateQuery>
{
    public PreviewCertificateQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.SampleName)
            .MaximumLength(200)
            .WithMessage("Sample participant name must be 200 characters or fewer.");
    }
}
