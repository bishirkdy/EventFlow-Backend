using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Certificates;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetMyCertificates;

public sealed class GetMyCertificatesQueryHandler(
    ICertificateRepository certificates,
    EventFlow.Security.Authentication.ICurrentUserService user)
    : IRequestHandler<GetMyCertificatesQuery, List<CertificateDto>>
{
    public async Task<List<CertificateDto>> Handle(
        GetMyCertificatesQuery query,
        CancellationToken cancellationToken)
    {
        var certificate = await certificates.GetByUserAsync(
            query.EventId,
            user.UserId,
            cancellationToken);

        if (certificate is null)
        {
            return [];
        }

        return [certificate.ToDto()];
    }
}
