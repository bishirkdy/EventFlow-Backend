using System.Net.Http.Json;using EventFlow.Operations.Application.Abstractions;
namespace EventFlow.Operations.Infrastructure.Services;
public sealed class EventScheduleClient(HttpClient h):IEventScheduleClient{public async Task<int> GetSessionCountAsync(Guid e,CancellationToken ct=default){var x=await h.GetFromJsonAsync<R>($"api/v1/session/{e:D}/sessions",ct);return x?.Data?.Count??0;}private sealed record R(bool IsSuccess,int StatusCode,List<object>? Data,string Message,List<string>? Errors);}
