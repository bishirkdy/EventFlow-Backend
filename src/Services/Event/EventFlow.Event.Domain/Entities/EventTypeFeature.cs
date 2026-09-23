using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities
{
    public sealed class EventTypeFeature : Entity
    {
        public Guid EventTypeId { get; private set; }
        public Guid FeatureId { get; private set; }
        public bool IsEnabledByDefault { get; private set; }
        
        //Navigation
        public Feature Feature { get; private set; } = null!;
        public EventType EventType { get; private set; } = null!;

        private EventTypeFeature()
        {
        }

        public EventTypeFeature(Guid eventTypeId,Guid featureId,bool isEnabledByDefault)
        {
            EventTypeId = eventTypeId;
            FeatureId = featureId;
            IsEnabledByDefault = isEnabledByDefault;
        }
    }
}
