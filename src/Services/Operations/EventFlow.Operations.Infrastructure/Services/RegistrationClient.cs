using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace EventFlow.Operations.Infrastructure.Services;

public sealed class RegistrationClient(
    HttpClient httpClient,
    IConfiguration configuration)
{
    private readonly string _registrationBaseUrl =
        configuration["Services:Registration:BaseUrl"]
        ?? throw new InvalidOperationException(
            "Services:Registration:BaseUrl is not configured.");

    public async Task<TicketVerificationResponse?> VerifyTicketAsync(
        string qrCodeValue,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_registrationBaseUrl.TrimEnd('/')}/api/internal/tickets/verify");

        request.Content = JsonContent.Create(
            new
            {
                qrCodeValue
            });

        var serviceKey = configuration["InternalService:Key"];

        if (!string.IsNullOrWhiteSpace(serviceKey))
        {
            request.Headers.Add(
                "X-Internal-Service-Key",
                serviceKey);
        }

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
            TicketVerificationResponse>(
                cancellationToken);
    }
}

public sealed record TicketVerificationResponse(
    Guid TicketId,
    Guid RegistrationId,
    Guid ParticipantId,
    Guid EventId,
    string TicketNumber,
    string ParticipantName,
    bool IsValid,
    bool IsActive);