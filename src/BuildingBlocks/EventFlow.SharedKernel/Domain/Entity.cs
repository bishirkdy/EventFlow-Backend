namespace EventFlow.SharedKernel.Domain;

public abstract class Entity
{
    // Generates a new ID and sets the creation time automatically.
    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    // Constructor used when an existing ID needs to be provided.
    protected Entity(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }


    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    // Updates the UpdatedAt timestamp.
    protected void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
