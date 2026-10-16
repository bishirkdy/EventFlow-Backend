using Confluent.Kafka;
using EventFlow.Messaging.Events;
using EventFlow.Messaging.Kafka;
using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventFlow.Operations.Infrastructure.Background;

public sealed class EventInvitationCreatedConsumer(IServiceScopeFactory scopeFactory, IOptions<KafkaOptions> kafkaOptions,
    IConfiguration configuration , ILogger<EventInvitationCreatedConsumer> logger): BackgroundService
{
    private const string Topic = "event-invitation-created";
    private const string GroupId = "operations-event-invitation";

    private const int PhotographerType = 1;
    private const int OrganizerType = 2;
    private const int AttendanceStaffType = 3;

    private readonly string _frontendBaseUrl = configuration["Frontend:BaseUrl"] ?? throw new InvalidOperationException("Frontend:BaseUrl is not configured.");

    protected override async Task ExecuteAsync( CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.BootstrapServers,
            GroupId = GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer =
            new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, error) =>
                {
                    logger.LogError(
                        "Kafka consumer error: {Reason}",
                        error.Reason);
                })
                .SetPartitionsAssignedHandler((_, partitions) =>
                {
                    logger.LogInformation(
                        "Kafka partitions assigned: {Partitions}",
                        string.Join(", ", partitions));
                })
                .SetPartitionsRevokedHandler((_, partitions) =>
                {
                    logger.LogWarning(
                        "Kafka partitions revoked: {Partitions}",
                        string.Join(", ", partitions));
                })
                .Build();

        consumer.Subscribe(Topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation(
                    "Waiting for event invitation Kafka message...");

                var result = consumer.Consume(stoppingToken);

                if (result?.Message?.Value is null)
                {
                    logger.LogWarning(
                        "Received Kafka message with empty value.");

                    continue;
                }

                EventInvitationCreatedEvent? message;

                try
                {
                    message =
                        JsonSerializer.Deserialize<EventInvitationCreatedEvent>(
                            result.Message.Value);
                }
                catch (JsonException exception)
                {
                    logger.LogError(
                        exception,
                        "Failed to deserialize event invitation Kafka message.");

                    continue;
                }

                if (message is null)
                {
                    logger.LogWarning(
                        "Received invalid event invitation event.");

                    continue;
                }

                using var scope = scopeFactory.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<IOperationsDbContext>();

                db.Notifications.Add(new Notification(
                    message.EventId,
                    null,
                    message.Email,
                    BuildSubject(message),
                    BuildEmailBody(message),
                    DateTime.UtcNow));

                await db.SaveChangesAsync(stoppingToken);

                consumer.Commit(result);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "Event invitation Kafka consumer stopped.");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Event invitation Kafka consumer stopped because of an unexpected error.");
        }
        finally
        {
            consumer.Close();

            logger.LogInformation(
                "Event invitation Kafka consumer closed.");
        }
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
