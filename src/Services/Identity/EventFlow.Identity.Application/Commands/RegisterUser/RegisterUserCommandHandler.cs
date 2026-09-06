using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Abstractions.Services;
using EventFlow.Identity.Application.DTOs.Authentication;
using EventFlow.Identity.Application.Exceptions;
using EventFlow.Identity.Domain.Entities;
using MediatR;

namespace EventFlow.Identity.Application.Commands.RegisterUser
{
    // Handles the user registration request and creates a new User domain entity.
    public sealed class RegisterUserCommandHandler(IUserRepository userRepository , IPasswordHasher passwordHasher) : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
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

            await userRepository.AddAsync(user,cancellationToken);

            return new RegisterUserResponse(user.Id,user.UserName,user.Email);
        }
    }
}
