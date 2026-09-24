using Microsoft.AspNetCore.Http;

namespace EventFlow.Event.Api.Requests.Venues
{
    public sealed class CreateVenueRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public int Capacity { get; set; }
        public IFormFile? Image { get; set; }
    }
}
