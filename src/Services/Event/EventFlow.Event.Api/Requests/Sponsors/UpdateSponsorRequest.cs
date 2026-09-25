using Microsoft.AspNetCore.Http;

namespace EventFlow.Event.Api.Requests.Sponsors;

public sealed class UpdateSponsorRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? WebsiteUrl { get; set; }
    public string SponsorLevel { get; set; } = "Partner";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public IFormFile? Logo { get; set; }
}
