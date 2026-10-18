using AutoMapper;
using EventFlow.Registration.Domain.Entities;

namespace EventFlow.Registration.Application.Common.Mappings;

public sealed class RegistrationFormMappingProfile : Profile
{
    public RegistrationFormMappingProfile()
    {
        CreateMap<RegistrationForm, RegistrationFormDto>()
            .ForMember(
                destination => destination.Fields,
                options => options.MapFrom(
                    source => source.Fields.OrderBy(field => field.DisplayOrder)));

        CreateMap<RegistrationFormField, RegistrationFormFieldDto>();
    }
}
