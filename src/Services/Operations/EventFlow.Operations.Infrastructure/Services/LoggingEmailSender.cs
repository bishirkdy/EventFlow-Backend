using Microsoft.Extensions.Logging;

namespace EventFlow.Operations.Infrastructure.Services;

public sealed class LoggingEmailSender(
    ILogger<LoggingEmailSender> logger)
{
    public Task SendAsync(
        string recipient,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Email queued for {Recipient}. Subject: {Subject}",
            recipient,
            subject);

        return Task.CompletedTask;
    }
}