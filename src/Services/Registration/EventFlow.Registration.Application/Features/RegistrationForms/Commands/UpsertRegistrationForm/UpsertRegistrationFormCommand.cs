using EventFlow.Registration.Application.Contracts.RegistrationForms;
using MediatR;
namespace EventFlow.Registration.Application.Features.RegistrationForms.Commands.UpsertRegistrationForm;

public sealed record UpsertRegistrationFormCommand(
    Guid EventId,
    UpsertRegistrationFormRequest Request
) : IRequest<RegistrationFormDto>;
