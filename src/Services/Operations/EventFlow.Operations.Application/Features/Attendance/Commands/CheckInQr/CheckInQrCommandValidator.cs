using EventFlow.Operations.Domain.Enums;
using FluentValidation;

namespace EventFlow.Operations.Application.Features.Attendance.Commands.CheckInQr;

public sealed class CheckInQrCommandValidator : AbstractValidator<CheckInQrCommand>
{
    public CheckInQrCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.QrCode)
            .NotEmpty()
            .WithMessage("QR code is required.");

        RuleFor(x => x.Method)
            .IsInEnum()
            .WithMessage("Invalid check-in method.");
    }
}
