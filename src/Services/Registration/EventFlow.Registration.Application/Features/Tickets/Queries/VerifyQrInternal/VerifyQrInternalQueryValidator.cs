using FluentValidation;

namespace EventFlow.Registration.Application.Features.Tickets.Queries.VerifyQrInternal;

public sealed class VerifyQrInternalQueryValidator : AbstractValidator<VerifyQrInternalQuery>
{
    public VerifyQrInternalQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.QrCodeValue)
            .NotEmpty()
            .WithMessage("QR code is required.");
    }
}
