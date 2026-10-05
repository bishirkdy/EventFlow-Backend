using FluentValidation;

namespace EventFlow.Registration.Application.Features.Tickets.Queries.VerifyQr;

public sealed class VerifyQrQueryValidator : AbstractValidator<VerifyQrQuery>
{
    public VerifyQrQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.QrCodeValue)
            .NotEmpty()
            .WithMessage("QR code is required.");
    }
}
