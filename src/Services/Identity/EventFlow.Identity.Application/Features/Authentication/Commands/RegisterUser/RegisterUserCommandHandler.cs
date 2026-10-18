using AutoMapper;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Abstractions.Services;
using EventFlow.Identity.Application.DTOs.Authentication;
using EventFlow.Identity.Domain.Entities;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Authentication.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper) : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var normailizedMail = request.Email.Trim().ToLowerInvariant();
        var normalizedUsername = request.UserName.Trim();

        var emailExists = await userRepository.ExistsByEmailAsync(normailizedMail, cancellationToken);

        if (emailExists)
        {
            throw new ConflictException("A user with this email already exists.");
        }

        var userNameExists = await userRepository.ExistsByUserNameAsync(normalizedUsername, cancellationToken);

        if (userNameExists)
        {
            throw new ConflictException("This username is already taken.");
        }

        var passwordHash = passwordHasher.Hash(request.Password);
        var user = new User(normalizedUsername, normailizedMail, request.FirstName.Trim(), request.LastName.Trim(), passwordHash);
        await userRepository.AddAsync(user, cancellationToken);

        var response = mapper.Map<RegisterUserResponse>(user);
        return response;
    }
}
