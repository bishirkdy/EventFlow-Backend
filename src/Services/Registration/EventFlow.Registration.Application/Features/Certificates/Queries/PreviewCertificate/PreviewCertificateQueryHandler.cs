using EventFlow.Registration.Application.Common.Certificates;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.PreviewCertificate;

public sealed class PreviewCertificateQueryHandler(
    ICertificateSettingsRepository settingsRepository,
    ICertificateSourceDataService sourceData,
    ICertificateDocumentService documentService,
    ICertificateTemplateBackgroundResolver templateBackground,
    ICertificateVerifyUrlProvider verifyUrlProvider,
    ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<PreviewCertificateQuery, CertificateDownloadDto>
{
    public async Task<CertificateDownloadDto> Handle(
        PreviewCertificateQuery query,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var now = DateTime.UtcNow;

        var settings = await settingsRepository.GetByEventIdAsync(
                query.EventId,
                cancellationToken)
            ?? new CertificateSettings
            {
                Id = Guid.NewGuid(),
                EventId = query.EventId,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

        var eventInfo = await sourceData.GetEventAsync(query.EventId, cancellationToken);

        if (eventInfo is null)
        {
            throw new NotFoundException("Event details could not be loaded.");
        }

        var useImageTemplate = settings.TemplateMode == CertificateTemplateMode.ImageTemplate;

        byte[]? background = null;
        if (useImageTemplate)
        {
            background = await templateBackground.ResolveAsync(settings, cancellationToken);
        }

        var sampleName = string.IsNullOrWhiteSpace(query.SampleName)
            ? "Sample Participant"
            : query.SampleName.Trim();

        const string certificateNumber = "CERT-PREVIEW";

        var documentData = new CertificateDocumentData(
            certificateNumber,
            sampleName,
            eventInfo.Name,
            CertificateEventText.FormatEventDates(eventInfo.StartDate, eventInfo.EndDate),
            settings.Title,
            settings.Subtitle,
            settings.ThemeColor,
            settings.SignatoryName,
            settings.SignatoryTitle,
            now,
            verifyUrlProvider.GetUrl(certificateNumber),
            useImageTemplate,
            settings.TemplateFields,
            background,
            settings.BackgroundImageWidth,
            settings.BackgroundImageHeight);

        var pdf = documentService.Generate(documentData);

        return new CertificateDownloadDto
        {
            CertificateNumber = certificateNumber,
            FileName = "certificate-preview.pdf",
            ContentType = "application/pdf",
            Content = pdf
        };
    }
}
