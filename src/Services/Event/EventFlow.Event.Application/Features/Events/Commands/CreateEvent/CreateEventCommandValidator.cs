using EventFlow.Event.Application.Common;
using FluentValidation;

namespace EventFlow.Event.Application.Features.Events.Commands.CreateEvent
{
    public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        private static readonly string[] AllowedImageTypes =
        [
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/gif"
        ];

        private const long MaxImageSizeBytes = 10 * 1024 * 1024;
        private const int MaxImageCount = 10;

        public CreateEventCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Event name is required.")
                .MaximumLength(200)
                .WithMessage("Event name cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Description cannot exceed 2000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.EventType)
                .NotEmpty()
                .WithMessage("Event type is required.")
                .MaximumLength(100)
                .WithMessage("Event type cannot exceed 100 characters.");

            RuleFor(x => x.SubType)
                .MaximumLength(100)
                .WithMessage("Event subtype cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.SubType));

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be greater than start date.");

            RuleFor(x => x.TimeZone)
                .NotEmpty()
                .WithMessage("Event time zone is required.")
                .MaximumLength(100)
                .WithMessage("Event time zone cannot exceed 100 characters.")
                .Must(BeValidTimeZone)
                .WithMessage("Event time zone is invalid.");

            RuleFor(x => x.Images)
                .NotEmpty()
                .WithMessage("At least one event image is required.")
                .Must(images => images.Count <= MaxImageCount)
                .WithMessage($"An event can have at most {MaxImageCount} images.");

            RuleForEach(x => x.Images).ChildRules(image =>
            {
                image.RuleFor(x => x.FileName)
                    .NotEmpty()
                    .MaximumLength(255)
                    .WithMessage("Image file name is invalid.");

                image.RuleFor(x => x.ContentType)
                    .Must(type => AllowedImageTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
                    .WithMessage("Only JPEG, PNG, WebP and GIF images are allowed.");

                image.RuleFor(x => x.Length)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(MaxImageSizeBytes)
                    .WithMessage("Each image must be greater than 0 bytes and no larger than 10 MB.");
            });
        }

        private static bool BeValidTimeZone(string timeZoneId)
            => TimeZoneHelper.TryGetTimeZone(timeZoneId, out _);
    }
}
