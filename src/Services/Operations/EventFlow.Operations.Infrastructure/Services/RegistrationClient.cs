using System.Net;
using System.Net.Http.Json;
using EventFlow.Operations.Application.Abstractions;
using Microsoft.Extensions.Configuration;
namespace EventFlow.Operations.Infrastructure.Services;

public sealed class RegistrationClient(HttpClient httpClient,IConfiguration configuration):IRegistrationClient
{
 public async Task<VerifiedTicket?> VerifyTicketAsync(Guid eventId,string qrCode,string? bearerToken,CancellationToken ct=default)
 {
  var baseUrl=configuration["Services:Registration:BaseUrl"]??throw new InvalidOperationException("Services:Registration:BaseUrl is not configured.");
  using var request=new HttpRequestMessage(HttpMethod.Get,$"{baseUrl.TrimEnd('/')}/api/v1/events/{eventId}/tickets/verify-internal?qrCode={Uri.EscapeDataString(qrCode.Trim())}");
  var key=configuration["InternalService:Key"]; if(!string.IsNullOrWhiteSpace(key))request.Headers.Add("X-Internal-Service-Key",key);
  using var response=await httpClient.SendAsync(request,ct);
  if(response.StatusCode==HttpStatusCode.NotFound)return null;
  var result=await response.Content.ReadFromJsonAsync<VerifyResponse>(ct);
  if(result?.Data is null)return new VerifiedTicket(Guid.Empty,Guid.Empty,Guid.Empty,false,result?.Message??"Ticket verification failed.");
  return new VerifiedTicket(result.Data.RegistrationId,result.Data.ParticipantId,result.Data.ParticipantUserId,result.IsSuccess&&result.Data.IsActive,result.Message);
 }
 private sealed record VerifyResponse(bool IsSuccess,int StatusCode,string? Message,TicketData? Data);
 private sealed record TicketData(Guid Id,Guid RegistrationId,Guid ParticipantId,Guid ParticipantUserId,string TicketNumber,string QrCodeValue,DateTime IssuedAtUtc,DateTime? RevokedAtUtc,bool IsActive);
}
