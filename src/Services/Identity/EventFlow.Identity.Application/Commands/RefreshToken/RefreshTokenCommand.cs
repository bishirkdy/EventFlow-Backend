using EventFlow.Identity.Application.DTOs;
using MediatR;

namespace EventFlow.Identity.Application.Commands.RefreshToken
{
    // Represents a request to issue new authentication tokens using a refresh token.
    public record RefreshTokenCommand(RefreshTokenDto RefreshToken) : IRequest<RefreshTokenResponseDto>;
}
