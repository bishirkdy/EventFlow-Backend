using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities;

public sealed class Feature : Entity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private Feature()
    {
    }

    public Feature(Guid id,string code,string name,string? description)
    {
        Id = id;
        Code = code;
        Name = name;
        Description = description;
        IsActive = true;
    }
}