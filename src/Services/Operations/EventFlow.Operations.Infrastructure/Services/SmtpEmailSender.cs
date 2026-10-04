using EventFlow.Operations.Application.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace EventFlow.Operations.Infrastructure.Services;

public sealed class SmtpEmailSender(
    IConfiguration configuration,
    ILogger<SmtpEmailSender> logger)
    : IEmailSender
{
    public async Task SendAsync(
        string recipientEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        var host = configuration["Smtp:Host"];

        if (string.IsNullOrWhiteSpace(host))
        {
            logger.LogInformation(
                "SMTP is not configured. Email to {Recipient} logged only. Subject: {Subject}",
                recipientEmail,
                subject);
            return;
        }

        var port = configuration.GetValue("Smtp:Port", 587);
        var useSsl = configuration.GetValue("Smtp:UseSsl", true);
        var username = configuration["Smtp:Username"];
        var password = configuration["Smtp:Password"];
        var from = configuration["Smtp:From"] ?? "no-reply@eventflow.local";

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(from));
        message.To.Add(MailboxAddress.Parse(recipientEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new MailKit.Net.Smtp.SmtpClient();

        var secureSocketOptions = useSsl
            ? SecureSocketOptions.StartTlsWhenAvailable
            : SecureSocketOptions.None;

        await client.ConnectAsync(host, port, secureSocketOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(username))
        {
            await client.AuthenticateAsync(
                username,
                password ?? string.Empty,
                cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
