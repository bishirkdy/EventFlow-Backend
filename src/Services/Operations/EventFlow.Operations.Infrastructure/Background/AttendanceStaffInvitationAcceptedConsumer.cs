using Confluent.Kafka;
using EventFlow.Messaging.Events;
using EventFlow.Messaging.Kafka;
using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Entities;
using EventFlow.Operations.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventFlow.Operations.Infrastructure.Background;

// Creates the scoped attendance staff assignment once the invited user has
// accepted the invitation. The event role is assigned by the Identity service;
// this consumer only owns the Operations side (the scope).
public sealed class AttendanceStaffInvitationAcceptedConsumer(IServiceScopeFactory scopeFactory,
    IOptions<KafkaOptions> kafkaOptions, ILogger<AttendanceStaffInvitationAcceptedConsumer> logger) : BackgroundService
{
    private const string Topic = "attendance-staff-invitation-accepted";
    private const string GroupId = "operations-attendance-staff-invitation";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                .Build();

        consumer.Subscribe(Topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation(
                    "Waiting for attendance staff invitation accepted Kafka message...");

                var result = consumer.Consume(stoppingToken);

                if (result?.Message?.Value is null)
                {
                    logger.LogWarning(
                        "Received Kafka message with empty value.");

                    continue;
                }

                AttendanceStaffInvitationAcceptedEvent? message;

                try
                {
                    message =
                        JsonSerializer.Deserialize<AttendanceStaffInvitationAcceptedEvent>(
                            result.Message.Value);
                }
                catch (JsonException exception)
                {
                    logger.LogError(
                        exception,
                        "Failed to deserialize attendance staff accepted Kafka message.");

                    continue;
                }

                if (message is null)
                {
                    logger.LogWarning(
                        "Received invalid attendance staff accepted event.");

                    continue;
                }

                using var scope = scopeFactory.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<IOperationsDbContext>();

                var scopeType = message.ScopeType is null
                    ? AttendanceScopeType.Event
                    : (AttendanceScopeType)message.ScopeType;

                var exists = await db.AttendanceStaffAssignments.AnyAsync(
                    x => x.EventId == message.EventId
                         && x.UserId == message.UserId
                         && x.ScopeType == scopeType
                         && x.ScopeId == message.ScopeId
                         && x.IsActive,
                    stoppingToken);

                if (!exists)
                {
                    db.AttendanceStaffAssignments.Add(new AttendanceStaffAssignment(
                        message.EventId,
                        message.UserId,
                        scopeType,
                        message.ScopeId));

                    await db.SaveChangesAsync(stoppingToken);

                    logger.LogInformation(
                        "Attendance staff assignment created for user {UserId} on event {EventId}.",
                        message.UserId,
                        message.EventId);
                }

                consumer.Commit(result);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "Attendance staff accepted Kafka consumer stopped.");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Attendance staff accepted Kafka consumer stopped because of an unexpected error.");
        }
        finally
        {
            consumer.Close();

            logger.LogInformation(
                "Attendance staff accepted Kafka consumer closed.");
        }
    }
}
