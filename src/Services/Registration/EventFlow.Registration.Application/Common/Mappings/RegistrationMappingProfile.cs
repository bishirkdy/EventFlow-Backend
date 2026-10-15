using AutoMapper;
using EventFlow.Registration.Domain.Entities;
using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Application.Common.Mappings;

public sealed class RegistrationMappingProfile : Profile
{
    public RegistrationMappingProfile()
    {
        // Registration -> read model (participant and ticket are mapped inline).
        CreateMap<RegistrationEntity, RegistrationDto>();

        // Participant -> read model.
        CreateMap<Participant, ParticipantDto>();

        // Ticket -> read model; the participant user id comes from the navigation.
        CreateMap<Ticket, TicketDto>()
            .ForMember(
                destination => destination.ParticipantUserId,
                options => options.MapFrom(
                    source => source.Participant != null
                        ? source.Participant.UserId
                        : Guid.Empty));
    }
}
