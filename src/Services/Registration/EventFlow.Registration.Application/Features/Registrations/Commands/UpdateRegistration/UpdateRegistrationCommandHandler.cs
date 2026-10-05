using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;
using EventFlow.Registration.Application.Features.Registrations;
using EventFlow.SharedKernel.Exceptions;
using FluentValidation.Results;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.UpdateRegistration;

public sealed class UpdateRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    IRegistrationFormRepository forms,
    EventFlow.Security.Authentication.ICurrentUserService user)
    : IRequestHandler<UpdateRegistrationCommand, RegistrationDto>
{
    public async Task<RegistrationDto> Handle(
        UpdateRegistrationCommand command,
        CancellationToken cancellationToken)
    {
        var registration = await registrations.GetByIdAsync(
            command.EventId,
            command.RegistrationId,
            userId: user.UserId,
            includeParticipant: true,
            includeAnswers: true,
            cancellationToken: cancellationToken);

        if (registration is null)
        {
            throw new NotFoundException("Registration not found.");
        }

        var form = await forms.GetByEventIdAsync(
            command.EventId,
            includeFields: true,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        var answerErrors = RegistrationFormAnswerValidator.Validate(
            form,
            command.Request.Answers);

        if (answerErrors.Count > 0)
        {
            throw new ValidationException(
                answerErrors.Select(
                    message => new ValidationFailure(string.Empty, message)));
        }

        if (registration.Status is
            RegistrationStatus.Cancelled or
            RegistrationStatus.Rejected)
        {
            throw new ConflictException("Registration cannot be edited.");
        }

        if (registration.Participant is null)
        {
            throw new NotFoundException("Participant not found.");
        }

        var now = DateTime.UtcNow;

        registration.Participant.FirstName =
            command.Request.FirstName.Trim();
        registration.Participant.LastName =
            command.Request.LastName.Trim();
        registration.Participant.Email =
            command.Request.Email.Trim();
        registration.Participant.Phone =
            command.Request.Phone?.Trim();
        registration.Participant.Organization =
            command.Request.Organization?.Trim();
        registration.Participant.Designation =
            command.Request.Designation?.Trim();
        registration.Participant.UpdatedAtUtc = now;
        registration.UpdatedAtUtc = now;

        registrations.RemoveAnswers(registration.Answers);

        foreach (var answer in command.Request.Answers)
        {
            registration.Answers.Add(
                new RegistrationAnswer
                {
                    Id = Guid.NewGuid(),
                    RegistrationId = registration.Id,
                    RegistrationFormFieldId = answer.Key,
                    Value = answer.Value ?? string.Empty
                });
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return registration.ToDto();
    }
}
