using System.Net.Http.Headers;
using EventFlow.Registration.Application.Abstractions.Services;

namespace EventFlow.Registration.Api.Services;

public sealed class CertificateSourceDataService(
    IHttpClientFactory factory,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration) : ICertificateSourceDataService
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
                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<EventPayload>(
                cancellationToken: cancellationToken);

            if (payload?.Data is null)
            {
                return null;
            }

            return new CertificateEventInfo(
                payload.Data.Id,
                payload.Data.Name,
                payload.Data.StartDate,
                payload.Data.EndDate,
                payload.Data.TimeZone);
        }
        catch
        {
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
        catch
        {
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
