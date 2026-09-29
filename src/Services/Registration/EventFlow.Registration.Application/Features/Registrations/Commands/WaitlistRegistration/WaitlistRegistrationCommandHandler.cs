using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.WaitlistRegistration;

public sealed class WaitlistRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        WaitlistRegistrationCommand,
        ApiResponse<RegistrationDto>>
{
    public async Task<ApiResponse<RegistrationDto>> Handle(
        WaitlistRegistrationCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<RegistrationDto>.Fail(
                ["You do not have permission."]);
        }

        var registration = await registrations.GetByIdAsync(
            command.EventId,
            command.RegistrationId,
            includeParticipant: true,
            includeTicket: true,
            cancellationToken: cancellationToken);

        if (registration is null)
        {
            return ApiResponse<RegistrationDto>.Fail(
                ["Registration not found."]);
        }

        if (registration.Status is
            RegistrationStatus.Approved or
            RegistrationStatus.Cancelled)
        {
            return ApiResponse<RegistrationDto>.Fail(
                ["Registration cannot be waitlisted."]);
        }

        if (registration.Status == RegistrationStatus.Waitlisted)
        {
            return ApiResponse<RegistrationDto>.Success(
                registration.ToDto(),
                "Already waitlisted.");
        }

        registration.Status = RegistrationStatus.Waitlisted;
        registration.WaitlistedAtUtc = DateTime.UtcNow;

        var currentWaitlistedCount =
            await registrations.CountByStatusAsync(
                command.EventId,
                RegistrationStatus.Waitlisted,
                cancellationToken);

        registration.WaitlistPosition =
            currentWaitlistedCount + 1;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<RegistrationDto>.Success(
            registration.ToDto(),
            "Registration moved to waitlist.");
    }
}
