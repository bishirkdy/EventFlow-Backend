using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using MediatR;
namespace EventFlow.Registration.Application.Features.Registrations.Commands.RejectRegistration;

public sealed record RejectRegistrationCommand(
    Guid EventId,
    Guid RegistrationId,
    RejectRegistrationRequest Request
) : IRequest<RegistrationDto>;
