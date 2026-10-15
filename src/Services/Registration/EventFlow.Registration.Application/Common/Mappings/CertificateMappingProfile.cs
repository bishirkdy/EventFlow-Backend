using AutoMapper;
using EventFlow.Registration.Domain.Entities;

namespace EventFlow.Registration.Application.Common.Mappings;

public sealed class CertificateMappingProfile : Profile
{
    public CertificateMappingProfile()
    {
        // Certificate -> read model (status is exposed as its name).
        CreateMap<Certificate, CertificateDto>()
            .ForMember(
                destination => destination.Status,
                options => options.MapFrom(source => source.Status.ToString()));
    }
}
