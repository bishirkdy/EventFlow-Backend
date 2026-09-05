using EventFlow.Identity.Application.DTOs;
using MediatR;

namespace EventFlow.Identity.Application.Commands.ChangePassword
{
    // Represents a request for an authenticated user to change their password.
    public record ChangePasswordCommand(ChangePasswordDto ChangePassword) : IRequest;
}
