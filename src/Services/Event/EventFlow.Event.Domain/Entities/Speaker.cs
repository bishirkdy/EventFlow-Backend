using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities;

public sealed class Speaker : Entity
{
    public Guid EventId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Bio { get; private set; }
    public string? Designation { get; private set; }
    public string? Organization { get; private set; }
    public string? Email { get; private set; }
    public string? ImageUrl { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    private Speaker() { }

    public Speaker(Guid eventId, string name, string? bio, string? designation, string? organization, string? email, string? imageUrl, int displayOrder = 0)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        Name = name;
        Bio = bio;
        Designation = designation;
        Organization = organization;
        Email = email;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public void Update(string name, string? bio, string? designation, string? organization, string? email, string? imageUrl, int displayOrder, bool isActive)
    {
        Name = name;
        Bio = bio;
        Designation = designation;
        Organization = organization;
        Email = email;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
