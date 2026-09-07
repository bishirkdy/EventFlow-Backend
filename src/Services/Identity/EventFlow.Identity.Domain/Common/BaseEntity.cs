namespace EventFlow.Identity.Domain.Common;

//Provides common properties to domain entities that we don't repeat
public abstract class BaseEntity
{
    protected BaseEntity()
    {
    }

    protected BaseEntity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    protected void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}