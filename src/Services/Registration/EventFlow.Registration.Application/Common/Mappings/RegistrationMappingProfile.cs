using AutoMapper;
using EventFlow.Registration.Domain.Entities;
using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Application.Common.Mappings;

public sealed class RegistrationMappingProfile : Profile
{
    public RegistrationMappingProfile()
    {
        CreateMap<RegistrationEntity, RegistrationDto>();

        CreateMap<Participant, ParticipantDto>();

        CreateMap<Ticket, TicketDto>()
            .ForMember(
                destination => destination.ParticipantUserId,
                options => options.MapFrom(
                    source => source.Participant != null
                        ? source.Participant.UserId
                        : Guid.Empty));
    }
}
