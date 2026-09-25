using Microsoft.AspNetCore.Http;

namespace EventFlow.Event.Api.Requests.Sponsors;

public sealed class CreateSponsorRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? WebsiteUrl { get; set; }
    public string SponsorLevel { get; set; } = "Partner";
    public int DisplayOrder { get; set; }
    public IFormFile? Logo { get; set; }
}
