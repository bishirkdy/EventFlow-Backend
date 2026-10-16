using EventFlow.Messaging.Events;
using EventFlow.Messaging.Kafka;
using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Entities;
using EventFlow.Operations.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventFlow.Operations.Infrastructure.Background;

// Creates the scoped attendance staff assignment once the invited user has
// accepted the invitation. The event role is assigned by the Identity service;
// this consumer only owns the Operations side (the scope).
public sealed class AttendanceStaffInvitationAcceptedConsumer(
    IServiceScopeFactory scopeFactory,
    IOptions<KafkaOptions> kafkaOptions,
    IKafkaProducer producer,
    ILoggerFactory loggerFactory)
    : KafkaConsumerBase<AttendanceStaffInvitationAcceptedEvent>(scopeFactory, kafkaOptions, producer, loggerFactory)
{
    protected override string Topic => "attendance-staff-invitation-accepted";

    protected override string GroupId => "operations-attendance-staff-invitation";

    protected override string ConsumerName => "operations-attendance-staff-invitation";

    protected override async Task HandleMessageAsync(
        AttendanceStaffInvitationAcceptedEvent message,
        IOperationsDbContext db,
        CancellationToken cancellationToken)
    {
        var scopeType = message.ScopeType is null
            ? AttendanceScopeType.Event
            : (AttendanceScopeType)message.ScopeType;

        var exists = await db.AttendanceStaffAssignments.AnyAsync(
            x => x.EventId == message.EventId
                 && x.UserId == message.UserId
                 && x.ScopeType == scopeType
                 && x.ScopeId == message.ScopeId
                 && x.IsActive,
            cancellationToken);

        if (exists)
        {
            return;
        }

        db.AttendanceStaffAssignments.Add(new AttendanceStaffAssignment(
            message.EventId,
            message.UserId,
            scopeType,
            message.ScopeId));
    }
}
