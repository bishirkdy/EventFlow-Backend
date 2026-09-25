using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities;

public sealed class Sponsor : Entity
{
    public Guid EventId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string? LogoUrl { get; private set; }
    public string SponsorLevel { get; private set; } = "Partner";
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    private Sponsor() { }

    public Sponsor(Guid eventId, string name, string? description, string? websiteUrl, string? logoUrl, string sponsorLevel, int displayOrder = 0)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        Name = name;
        Description = description;
        WebsiteUrl = websiteUrl;
        LogoUrl = logoUrl;
        SponsorLevel = sponsorLevel;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public void Update(string name, string? description, string? websiteUrl, string? logoUrl, string sponsorLevel, int displayOrder, bool isActive)
    {
        Name = name;
        Description = description;
        WebsiteUrl = websiteUrl;
        LogoUrl = logoUrl;
        SponsorLevel = sponsorLevel;
        DisplayOrder = displayOrder;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
