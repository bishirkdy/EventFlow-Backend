using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateSettings;

public sealed class GetCertificateSettingsQueryValidator : AbstractValidator<GetCertificateSettingsQuery>
{
    public GetCertificateSettingsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
