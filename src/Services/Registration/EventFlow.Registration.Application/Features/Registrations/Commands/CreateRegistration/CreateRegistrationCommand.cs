using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.CreateRegistration;

public sealed record CreateRegistrationCommand(Guid EventId,CreateRegistrationRequest Request) : 
    IRequest<RegistrationDto>;
