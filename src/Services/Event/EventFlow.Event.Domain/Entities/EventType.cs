using EventFlow.Event.Domain.Common;
using EventFlow.Event.Domain.Enums;

namespace EventFlow.Event.Domain.Entities
{
    public sealed class EventType : Entity
    {

        public EventTypeCode Code { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        private EventType()
        {
        }

        public EventType(Guid id,EventTypeCode code,string name,string? description)
        {
            Id = id;
            Code = code;
            Name = name;
            Description = description;
            IsActive = true;
        }
    }
}
