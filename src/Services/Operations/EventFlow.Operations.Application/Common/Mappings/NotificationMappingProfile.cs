using AutoMapper;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Entities;

namespace EventFlow.Operations.Application.Common.Mappings;

public sealed class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        // Notification entity -> read model.
        CreateMap<Notification, NotificationDto>();
    }
}
