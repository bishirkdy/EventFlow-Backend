using AutoMapper;
using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;
using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Application.Common.Mappings;

public sealed class RegistrationMappingProfile : Profile
{
    public RegistrationMappingProfile()
    {
        CreateMap<RegistrationEntity, RegistrationDto>();

        CreateMap<Participant, ParticipantDto>()
            .ForMember(
                destination => destination.ScopeType,
                options => options.MapFrom(
                    source => source.Registration == null
                        ? RegistrationScope.Event
                        : source.Registration.ScopeType))
            .ForMember(
                destination => destination.ScopeId,
                options => options.MapFrom(
                    source => source.Registration == null ? null : source.Registration.ScopeId));

        CreateMap<Ticket, TicketDto>()
            .ForMember(
                destination => destination.ParticipantUserId,
                options => options.MapFrom(
                    source => source.Participant != null
                        ? source.Participant.UserId
                        : Guid.Empty));
    }
}
