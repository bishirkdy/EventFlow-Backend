using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventFlow.Operations.Infrastructure.Background;

public sealed class NotificationWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<NotificationWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "EventFlow notification worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();

                // Process queued notifications here.
                //
                // Keep this worker responsible only for orchestration.
                // The actual notification query/sending logic belongs
                // to application/infrastructure services.

                await Task.Delay(
                    TimeSpan.FromSeconds(10),
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Error while processing notification queue.");

                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    stoppingToken);
            }
        }

        logger.LogInformation(
            "EventFlow notification worker stopped.");
    }
}