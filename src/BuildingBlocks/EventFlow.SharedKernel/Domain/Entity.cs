
namespace EventFlow.SharedKernel.Domain
{
    public abstract class Entity
    {
        protected Entity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        protected Entity(Guid id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }

        protected void SetUpdatedAt()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
