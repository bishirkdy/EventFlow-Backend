using AutoMapper;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Entities;

namespace EventFlow.Operations.Application.Common.Mappings;

public sealed class AttendanceStaffMappingProfile : Profile
{
    public AttendanceStaffMappingProfile()
    {
        // Email is resolved from Identity, not the domain entity.
        CreateMap<AttendanceStaffAssignment, AttendanceStaffDto>()
            .ForCtorParam(
                nameof(AttendanceStaffDto.Email),
                opt => opt.MapFrom(_ => string.Empty));
    }
}
