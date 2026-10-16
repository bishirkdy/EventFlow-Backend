using System.Text.Json;
using EventFlow.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Messaging.Tests;

public sealed class OutboxEnqueueTests
{
    [Fact]
    public async Task Enqueue_persists_a_pending_message_with_the_business_change()
    {
        await using var provider = TestServiceProvider.Build("enqueue", new FakeKafkaProducer());

        var messageId = Guid.NewGuid();
        var eventContextId = Guid.NewGuid();

        using (var scope = provider.CreateScope())
        {
            var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();

            outbox.Enqueue("test-topic", new TestEvent
            {
                MessageId = messageId,
                EventId = eventContextId,
                EventContextId = eventContextId,
                Name = "hello",
                OccurredAtUtc = DateTime.UtcNow
            });

            var db = scope.ServiceProvider.GetRequiredService<TestOutboxDbContext>();
            await db.SaveChangesAsync();
        }

        using (var scope = provider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TestOutboxDbContext>();
            var stored = await db.OutboxMessages.SingleAsync();

            Assert.Equal("test-topic", stored.Topic);
            Assert.Equal(messageId, stored.MessageId);
            Assert.Equal(eventContextId, stored.EventContextId);
            Assert.Equal(OutboxStatus.Pending, stored.Status);
            Assert.Equal(0, stored.Attempts);

            var payload = JsonSerializer.Deserialize<TestEvent>(stored.Payload);
            Assert.NotNull(payload);
            Assert.Equal("hello", payload!.Name);
        }
    }
}
