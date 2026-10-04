using EventFlow.Event.Domain.Enums;
using EventFlow.SharedKernel.Domain;

namespace EventFlow.Event.Domain.Entities
{
    public class Feedback : Entity
    {
        private Feedback()
        {
        }

        public Feedback(
            Guid eventId,
            Guid participantUserId,
            FeedbackTargetType targetType,
            Guid? targetId,
            int rating,
            string? comment)
        {
            EventId = eventId;
            ParticipantUserId = participantUserId;
            TargetType = targetType;
            TargetId = targetId;
            Rating = rating;
            Comment = comment;
            SubmittedAtUtc = DateTime.UtcNow;
        }

        public Guid EventId { get; private set; }
        public Guid ParticipantUserId { get; private set; }
        public FeedbackTargetType TargetType { get; private set; }
        public Guid? TargetId { get; private set; }
        public int Rating { get; private set; }
        public string? Comment { get; private set; }
        public DateTime SubmittedAtUtc { get; private set; }
    }
}
