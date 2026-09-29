using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using MediatR;
using EventFlow.Contracts.Common;
namespace EventFlow.Registration.Application.Features.Registrations.Commands.PromoteWaitlistedRegistration;

public sealed record PromoteWaitlistedRegistrationCommand(
    Guid EventId,
    Guid RegistrationId
) : IRequest<ApiResponse<RegistrationDto>>;
