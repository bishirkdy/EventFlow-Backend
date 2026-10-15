using AutoMapper;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Entities;

namespace EventFlow.Operations.Application.Common.Mappings;

public sealed class AttendanceMappingProfile : Profile
{
    public AttendanceMappingProfile()
    {
        // Attendance record -> read model.
        CreateMap<AttendanceRecord, AttendanceDto>();
    }
}
