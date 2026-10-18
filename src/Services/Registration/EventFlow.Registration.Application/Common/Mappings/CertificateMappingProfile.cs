using AutoMapper;
using EventFlow.Registration.Domain.Entities;

namespace EventFlow.Registration.Application.Common.Mappings;

public sealed class CertificateMappingProfile : Profile
{
    public CertificateMappingProfile()
    {
        CreateMap<Certificate, CertificateDto>()
            .ForMember(
                destination => destination.Status,
                options => options.MapFrom(source => source.Status.ToString()));
    }
}
