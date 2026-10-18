using EventFlow.Identity.Application.DTOs;
using EventFlow.Identity.Application.DTOs.Authentication;
using MediatR;

namespace EventFlow.Identity.Application.Features.Authentication.Commands.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<LoginResponse>;
