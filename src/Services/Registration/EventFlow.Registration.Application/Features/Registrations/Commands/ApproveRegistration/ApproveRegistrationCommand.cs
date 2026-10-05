using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.ApproveRegistration;

public sealed record ApproveRegistrationCommand(
    Guid EventId,
    Guid RegistrationId
) : IRequest<RegistrationDto>;
