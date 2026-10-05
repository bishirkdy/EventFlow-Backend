using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.ListCertificates;

public sealed class ListCertificatesQueryValidator : AbstractValidator<ListCertificatesQuery>
{
    public ListCertificatesQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
