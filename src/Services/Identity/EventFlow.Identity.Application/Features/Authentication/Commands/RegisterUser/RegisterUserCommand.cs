using EventFlow.Identity.Application.DTOs.Authentication;
using MediatR;

namespace EventFlow.Identity.Application.Features.Authentication.Commands.RegisterUser;

public sealed record RegisterUserCommand(string UserName, string Email, string Password, string FirstName, string LastName) : IRequest<RegisterUserResponse>;
