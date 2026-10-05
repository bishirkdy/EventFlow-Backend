using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Security.Authentication;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetMyCertificatesAcrossEvents;

public sealed class GetMyCertificatesAcrossEventsQueryHandler(
    ICertificateRepository certificates,
    ICurrentUserService user)
    : IRequestHandler<GetMyCertificatesAcrossEventsQuery, List<CertificateDto>>
{
    public async Task<List<CertificateDto>> Handle(
        GetMyCertificatesAcrossEventsQuery query,
        CancellationToken cancellationToken)
    {
        var items = await certificates.GetForUserAsync(
            user.UserId,
            cancellationToken);

        return items.Select(x => x.ToDto()).ToList();
    }
}
