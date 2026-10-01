

namespace EventFlow.SharedKernel.Domain
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        protected AggregateRoot()
        {
        }

        protected AggregateRoot(Guid id)
            : base(id)
        {
        }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            ArgumentNullException.ThrowIfNull(domainEvent);

            _domainEvents.Add(domainEvent);
        }

        public IReadOnlyCollection<IDomainEvent> GetDomainEvents()
        {
            return _domainEvents.AsReadOnly();
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
