using Microsoft.AspNetCore.Http;

namespace EventFlow.Event.Api.Requests.Events
{
    public sealed class CreateEventRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? SubType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeZone { get; set; } = string.Empty;
        public List<IFormFile> Images { get; set; } = [];
    }
}
