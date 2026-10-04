using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Security.Authentication;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetMyCertificatesAcrossEvents;

public sealed class GetMyCertificatesAcrossEventsQueryHandler(
    ICertificateRepository certificates,
    ICurrentUserService user)
    : IRequestHandler<GetMyCertificatesAcrossEventsQuery, ApiResponse<List<CertificateDto>>>
{
    public async Task<ApiResponse<List<CertificateDto>>> Handle(
        GetMyCertificatesAcrossEventsQuery query,
        CancellationToken cancellationToken)
    {
        var items = await certificates.GetForUserAsync(
            user.UserId,
            cancellationToken);

        return ApiResponse<List<CertificateDto>>.Success(
            items.Select(x => x.ToDto()).ToList(),
            items.Count == 0
                ? "No certificates have been issued for you yet."
                : "Certificates found.");
    }
}
