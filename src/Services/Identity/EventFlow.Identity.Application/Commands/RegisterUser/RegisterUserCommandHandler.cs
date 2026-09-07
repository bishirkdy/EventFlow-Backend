using AutoMapper;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Abstractions.Services;
using EventFlow.Identity.Application.DTOs.Authentication;
using EventFlow.Identity.Application.Exceptions;
using EventFlow.Identity.Domain.Entities;
using MediatR;

namespace EventFlow.Identity.Application.Commands.RegisterUser
{
    // Handles the user registration request and creates a new User domain entity.
    public sealed class RegisterUserCommandHandler(IUserRepository userRepository , IPasswordHasher passwordHasher , IMapper mapper) : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Normalize the email and username before checking and storing it.
            var normailizedMail = request.Email.Trim().ToLowerInvariant();
            var normalizedUsername = request.UserName.Trim();

            // Check whether a user already exists with this email.
            var emailExists = await userRepository.ExistsByEmailAsync(normailizedMail, cancellationToken);

            if (emailExists)
            {
                throw new ConflictException("A user with this email already exists.");
            }

            // Check whether the username is already taken.
            var userNameExists = await userRepository.ExistsByUserNameAsync(normalizedUsername, cancellationToken);

            if (userNameExists)
            {
                throw new ConflictException("This username is already taken.");
            }

            // Hash the password before storing it in the database.
            var passwordHash = passwordHasher.Hash(request.Password);
            var user = new User(normalizedUsername, normailizedMail, request.FirstName.Trim(), request.LastName.Trim(), passwordHash);
            await userRepository.AddAsync(user,cancellationToken);

            var response = mapper.Map<RegisterUserResponse>(user);
            return response;
        }
    }
}
