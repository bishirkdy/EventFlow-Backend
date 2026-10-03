using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Certificates;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.ListCertificates;

public sealed class ListCertificatesQueryHandler(
    ICertificateRepository certificates,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<ListCertificatesQuery, ApiResponse<List<CertificateDto>>>
{
    public async Task<ApiResponse<List<CertificateDto>>> Handle(
        ListCertificatesQuery query,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<List<CertificateDto>>.Fail(
                ["You do not have permission."]);
        }

        var rows = await certificates.GetByEventIdAsync(
            query.EventId,
            cancellationToken);

        var dto = rows
            .OrderByDescending(x => x.IssuedAtUtc)
            .Select(x => x.ToDto())
            .ToList();

        return ApiResponse<List<CertificateDto>>.Success(
            dto,
            "Certificates loaded.");
    }
}
