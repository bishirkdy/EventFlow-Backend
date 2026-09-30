using EventFlow.Contracts.Common;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.UpdateRegistration;

public sealed record UpdateRegistrationCommand(
    Guid EventId,
    Guid RegistrationId,
    UpdateRegistrationRequest Request
) : IRequest<ApiResponse<RegistrationDto>>;
