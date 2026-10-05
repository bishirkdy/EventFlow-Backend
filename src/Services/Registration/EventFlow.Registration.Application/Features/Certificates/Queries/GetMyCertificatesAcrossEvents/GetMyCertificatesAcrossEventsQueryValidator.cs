using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetMyCertificatesAcrossEvents;

public sealed class GetMyCertificatesAcrossEventsQueryValidator
    : AbstractValidator<GetMyCertificatesAcrossEventsQuery>
{
    public GetMyCertificatesAcrossEventsQueryValidator()
    {
    }
}
