using Confluent.Kafka;
using EventFlow.Messaging.Events;
using EventFlow.Messaging.Kafka;
using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Entities;
using EventFlow.Operations.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventFlow.Operations.Infrastructure.Background;

public sealed class PhotographerInvitationCreatedConsumer(IServiceScopeFactory scopeFactory, IOptions<KafkaOptions> kafkaOptions,
    ILogger<PhotographerInvitationCreatedConsumer> logger): BackgroundService
{
    private const string Topic = "photographer-invitation-created";
    private const string GroupId = "operations-photographer-invitation";

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
                    "Waiting for photographer invitation Kafka message...");

                var result = consumer.Consume(stoppingToken);

                if (result?.Message?.Value is null)
                {
                    logger.LogWarning(
                        "Received Kafka message with empty value.");

                    continue;
                }


                PhotographerInvitationCreatedEvent? message;

                try
                {
                    message =
                        JsonSerializer.Deserialize<PhotographerInvitationCreatedEvent>(
                            result.Message.Value);
                }
                catch (JsonException exception)
                {
                    logger.LogError(
                        exception,
                        "Failed to deserialize photographer invitation Kafka message.");

                    continue;
                }

                if (message is null)
                {
                    logger.LogWarning(
                        "Received invalid photographer invitation event.");

                    continue;
                }

                using var scope = scopeFactory.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<IOperationsDbContext>();

                db.Notifications.Add(new Notification
                {
                    EventId = message.EventId,
                    RecipientEmail = message.Email,
                    Subject = "Photographer Invitation",
                    Body = BuildEmailBody(message),
                    Status = NotificationStatus.Pending,
                    ScheduledAtUtc = DateTime.UtcNow,
                    CreatedAtUtc = DateTime.UtcNow,
                    AttemptCount = 0
                });

                await db.SaveChangesAsync(stoppingToken);

                consumer.Commit(result);

            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "Photographer invitation Kafka consumer stopped.");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Photographer invitation Kafka consumer stopped because of an unexpected error.");
        }
        finally
        {
            consumer.Close();

            logger.LogInformation(
                "Photographer invitation Kafka consumer closed.");
        }
    }

    private static string BuildEmailBody(
        PhotographerInvitationCreatedEvent message)
    {
        return $"""
            You have been invited to join an event as a photographer.

            Invitation Token:
            {message.Token}

            This invitation expires on:
            {message.ExpiresAt:u}

            Please use the invitation link to accept the invitation.
            """;
    }
}