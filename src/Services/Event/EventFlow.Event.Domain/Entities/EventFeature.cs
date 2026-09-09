

using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities
{
    public sealed class EventFeature : Entity
    {
        public Guid EventId { get; private set; }
        public Guid FeatureId { get; private set; }
        public bool IsEnabled { get; private set; }

        private EventFeature()
        {
            // Required by EF Core
        }

        public EventFeature(Guid eventId,Guid featureId,bool isEnabled = true)
        {
            EventId = eventId;
            FeatureId = featureId;
            IsEnabled = isEnabled;
        }

        public void Enable()
        {
            IsEnabled = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Disable()
        {
            IsEnabled = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
