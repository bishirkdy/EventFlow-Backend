using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using MediatR;

using EventFlow.Contracts.Common;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.CreateRegistration;

public sealed record CreateRegistrationCommand(Guid EventId,CreateRegistrationRequest Request) : 
    IRequest<ApiResponse<RegistrationDto>>;
