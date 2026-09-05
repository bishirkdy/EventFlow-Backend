using EventFlow.Identity.Application.DTOs;
using MediatR;

namespace EventFlow.Identity.Application.Commands.ResetPassword
{
    // Represents a request to set a new password using a valid reset token.
    public record ResetPasswordCommand(ResetPasswordDto ResetPassword) : IRequest;
}
