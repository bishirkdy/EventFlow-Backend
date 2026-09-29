using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using MediatR;
using EventFlow.Contracts.Common;
namespace EventFlow.Registration.Application.Features.Registrations.Commands.RejectRegistration;

public sealed record RejectRegistrationCommand(
    Guid EventId,
    Guid RegistrationId,
    RejectRegistrationRequest Request
) : IRequest<ApiResponse<RegistrationDto>>;
