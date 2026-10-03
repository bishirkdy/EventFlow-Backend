using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Contracts.Certificates;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.DownloadCertificate;

public sealed class DownloadCertificateQueryHandler(
    ICertificateRepository certificates,
    ICertificateFileStore fileStore,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<DownloadCertificateQuery, ApiResponse<CertificateDownloadDto>>
{
    public async Task<ApiResponse<CertificateDownloadDto>> Handle(
        DownloadCertificateQuery query,
        CancellationToken cancellationToken)
    {
        var certificate = await certificates.GetByIdAsync(
            query.EventId,
            query.CertificateId,
            cancellationToken);

        if (certificate is null)
        {
            return ApiResponse<CertificateDownloadDto>.Fail(
                ["Certificate not found."]);
        }

        var isOwner = certificate.UserId == user.UserId;

        if (!isOwner &&
            !await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<CertificateDownloadDto>.Fail(
                ["You do not have permission."]);
        }

        await using var content = await fileStore.OpenReadAsync(
            query.EventId,
            certificate.DocumentFileName,
            cancellationToken);

        if (content is null)
        {
            return ApiResponse<CertificateDownloadDto>.Fail(
                ["Certificate file could not be found."]);
        }

        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);

        return ApiResponse<CertificateDownloadDto>.Success(
            new CertificateDownloadDto
            {
                CertificateNumber = certificate.CertificateNumber,
                FileName = $"{certificate.CertificateNumber}.pdf",
                ContentType = "application/pdf",
                Content = buffer.ToArray()
            },
            "Certificate downloaded.");
    }
}
