using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using MediatR;
namespace EventFlow.Registration.Application.Features.Registrations.Commands.CancelRegistration;

public sealed record CancelRegistrationCommand(
    Guid EventId,
    Guid RegistrationId,
    CancelRegistrationRequest Request
) : IRequest<RegistrationDto>;
