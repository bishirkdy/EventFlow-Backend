using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Certificates;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetMyCertificates;

public sealed class GetMyCertificatesQueryHandler(
    ICertificateRepository certificates,
    EventFlow.Security.Authentication.ICurrentUserService user)
    : IRequestHandler<GetMyCertificatesQuery, ApiResponse<List<CertificateDto>>>
{
    public async Task<ApiResponse<List<CertificateDto>>> Handle(
        GetMyCertificatesQuery query,
        CancellationToken cancellationToken)
    {
        var certificate = await certificates.GetByUserAsync(
            query.EventId,
            user.UserId,
            cancellationToken);

        if (certificate is null)
        {
            return ApiResponse<List<CertificateDto>>.Success(
                [],
                "No certificate has been issued for you yet.");
        }

        return ApiResponse<List<CertificateDto>>.Success(
            [certificate.ToDto()],
            "Certificate found.");
    }
}
