using EventFlow.Identity.Application.DTOs.Authentication;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.RefreshAccessToken;

public sealed record RefreshAccessTokenCommand(string RefreshToken) : IRequest<LoginResponse>;
