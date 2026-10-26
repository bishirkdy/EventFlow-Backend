using EventFlow.Messaging.Events;
using EventFlow.Messaging.Kafka;
using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventFlow.Operations.Infrastructure.Background;

// Turns an invitation-created event into a queued notification email.
public sealed class EventInvitationCreatedConsumer : KafkaConsumerBase<EventInvitationCreatedEvent>
{
    private const int PhotographerType = 1;
    private const int OrganizerType = 2;
    private const int AttendanceStaffType = 3;

    private readonly string _frontendBaseUrl;

    public EventInvitationCreatedConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<KafkaOptions> kafkaOptions,
        IKafkaProducer producer,
        ILoggerFactory loggerFactory,
        IConfiguration configuration,
        IHostEnvironment environment)
        : base(scopeFactory, kafkaOptions, producer, loggerFactory)
    {
        _frontendBaseUrl = ResolveFrontendBaseUrl(
            configuration["Frontend:BaseUrl"],
            environment.IsDevelopment());

        loggerFactory
            .CreateLogger<EventInvitationCreatedConsumer>()
            .LogInformation(
                "Invitation links will use frontend base URL {FrontendBaseUrl}.",
                _frontendBaseUrl);
    }

    internal static string ResolveFrontendBaseUrl(string? configuredBaseUrl, bool isDevelopment)
    {
        if (string.IsNullOrWhiteSpace(configuredBaseUrl) ||
            !Uri.TryCreate(configuredBaseUrl.Trim(), UriKind.Absolute, out var frontendUri) ||
            (frontendUri.Scheme != Uri.UriSchemeHttp && frontendUri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                "Frontend:BaseUrl must be configured as an absolute HTTP or HTTPS URL.");
        }

        if (!isDevelopment &&
            (frontendUri.Scheme != Uri.UriSchemeHttps || frontendUri.IsLoopback))
        {
            throw new InvalidOperationException(
                "Frontend:BaseUrl must be a public HTTPS URL outside Development.");
        }

        return configuredBaseUrl.Trim().TrimEnd('/');
    }

    protected override string Topic => "event-invitation-created";

    protected override string GroupId => "operations-event-invitation";

    protected override string ConsumerName => "operations-event-invitation";

    protected override Task HandleMessageAsync(
        EventInvitationCreatedEvent message,
        IOperationsDbContext db,
        CancellationToken cancellationToken)
    {
        db.Notifications.Add(new Notification(
            message.EventId,
            null,
            message.Email,
            BuildSubject(message),
            BuildEmailBody(message),
            DateTime.UtcNow));

        return Task.CompletedTask;
    }

    private static string BuildSubject(EventInvitationCreatedEvent message) =>
        message.InvitationType switch
        {
            OrganizerType => "Organizer Invitation",
            AttendanceStaffType => "Attendance Staff Invitation",
            _ => "Photographer Invitation"
        };

    private string BuildEmailBody(EventInvitationCreatedEvent message)
    {
        var (linkSegment, roleDescription) = message.InvitationType switch
        {
            OrganizerType => ("organizer-invitation", "an organizer"),
            AttendanceStaffType => ("attendance-staff-invitation", "an attendance staff member"),
            _ => ("photographer-invitation", "a photographer")
        };

        var invitationLink =
            $"{_frontendBaseUrl.TrimEnd('/')}/{linkSegment}/{Uri.EscapeDataString(message.Token)}";

        return $"""
        You have been invited to join an event as {roleDescription}.

        Accept your invitation using this link:
        {invitationLink}
         
        This invitation expires on:
        {message.ExpiresAt:u}
        """;
    }
}
