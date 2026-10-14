using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.DeleteCertificateTemplateImage;

public sealed class DeleteCertificateTemplateImageCommandValidator
    : AbstractValidator<DeleteCertificateTemplateImageCommand>
{
    public DeleteCertificateTemplateImageCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
