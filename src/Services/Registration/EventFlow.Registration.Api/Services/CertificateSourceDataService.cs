using EventFlow.Registration.Application.Abstractions.Services;
using System.Net.Http.Headers;

namespace EventFlow.Registration.Api.Services;

public sealed class CertificateSourceDataService(
    IHttpClientFactory factory,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration,
    ILogger<CertificateSourceDataService> logger) : ICertificateSourceDataService
{
    public async Task<CertificateEventInfo?> GetEventAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = factory.CreateClient("EventService");

            using var request = CreateRequest(
                HttpMethod.Get,
                $"api/v1/events/{eventId}");

            var response = await client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Loading event {EventId} for certificates failed with status {StatusCode}.",
                    eventId,
                    response.StatusCode);
                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<EventPayload>(
                cancellationToken: cancellationToken);

            if (payload?.Data is null)
            {
                logger.LogWarning(
                    "Loading event {EventId} for certificates returned no data.",
                    eventId);
                return null;
            }

            return new CertificateEventInfo(
                payload.Data.Id,
                payload.Data.Name,
                payload.Data.StartDate,
                payload.Data.EndDate,
                payload.Data.TimeZone);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load event {EventId} for certificates.", eventId);
            return null;
        }
    }

    public async Task<double?> GetAttendancePercentAsync(
        Guid eventId,
        Guid participantUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = factory.CreateClient("OperationsService");

            using var request = CreateRequest(
                HttpMethod.Get,
                $"api/v1/operations/events/{eventId}/attendance/history/{participantUserId}");

            var response = await client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Loading attendance for user {UserId} in event {EventId} failed with status {StatusCode}.",
                    participantUserId,
                    eventId,
                    response.StatusCode);
                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<AttendancePayload>(
                cancellationToken: cancellationToken);

            if (payload is not { IsSuccess: true })
            {
                return null;
            }

            return payload.Data?.AttendancePercentage;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Failed to load attendance for user {UserId} in event {EventId}.",
                participantUserId,
                eventId);
            return null;
        }
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);

        var incoming = httpContextAccessor.HttpContext?.Request;

        var authorization = incoming is null
            ? null
            : incoming.Headers["Authorization"].ToString();
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            request.Headers.Authorization =
                AuthenticationHeaderValue.Parse(authorization);
        }

        if (incoming?.Cookies.TryGetValue("accessToken", out var cookie) == true &&
            !string.IsNullOrWhiteSpace(cookie))
        {
            request.Headers.TryAddWithoutValidation(
                "Cookie",
                $"accessToken={cookie}");
        }

        var serviceKey = configuration["InternalService:Key"];
        if (!string.IsNullOrWhiteSpace(serviceKey))
        {
            request.Headers.TryAddWithoutValidation(
                "X-Internal-Service-Key",
                serviceKey);
        }

        return request;
    }

    //Objects
    private sealed class EventPayload
    {
        public bool IsSuccess { get; set; }
        public EventData? Data { get; set; }
    }

    private sealed class EventData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeZone { get; set; } = "";
    }

    private sealed class AttendancePayload
    {
        public bool IsSuccess { get; set; }
        public AttendanceData? Data { get; set; }
    }

    private sealed class AttendanceData
    {
        public double AttendancePercentage { get; set; }
    }
}
