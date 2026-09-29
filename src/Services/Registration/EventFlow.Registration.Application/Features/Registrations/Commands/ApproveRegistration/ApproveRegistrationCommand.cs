using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using MediatR;

using EventFlow.Contracts.Common;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.ApproveRegistration;

public sealed record ApproveRegistrationCommand(
    Guid EventId,
    Guid RegistrationId
) : IRequest<ApiResponse<RegistrationDto>>;
