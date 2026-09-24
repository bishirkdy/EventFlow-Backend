using Microsoft.AspNetCore.Http;

namespace EventFlow.Event.Api.Requests.Sessions
{
    public sealed class CreateSessionRequest
    {
        public Guid SectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SessionType { get; set; } = string.Empty;
        public int? Capacity { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Guid? VenueId { get; set; }
        public IFormFile? Image { get; set; }
    }
}
