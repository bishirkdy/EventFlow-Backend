using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using MediatR;

namespace EventFlow.Identity.Application.Commands.RegisterUser
{
    // Handles the user registration request and creates a new User domain entity.
    public class RegisterUserCommandHandler(IUserRepository userRepository) : IRequestHandler<RegisterUserCommand , Guid>
    {
        public async Task<Guid> Handle(RegisterUserCommand request , CancellationToken cancellationToken)
        {
            var user = new User(request.User.UserName, request.User.UserName, request.User.FirstName, request.User.LastName);
            await userRepository.AddAsync(user , cancellationToken);
            return user.Id;
        }
    }
}
