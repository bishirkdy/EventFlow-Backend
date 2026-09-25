using Microsoft.AspNetCore.Http;

namespace EventFlow.Event.Api.Requests.Speakers;

public sealed class CreateSpeakerRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Designation { get; set; }
    public string? Organization { get; set; }
    public string? Email { get; set; }
    public int DisplayOrder { get; set; }
    public IFormFile? Image { get; set; }
}
