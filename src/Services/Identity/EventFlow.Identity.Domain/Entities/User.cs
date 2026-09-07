using EventFlow.Identity.Domain.Common;

namespace EventFlow.Identity.Domain.Entities
{
    //It contains user data and user state, but not authentication workflows.
    public class User : BaseEntity
    {
        private User()
        {
        }

        public User(string userName,string email,string firstName,string lastName,string passwordHash)
        {
            Id = Guid.NewGuid();

            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;

            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public string UserName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public bool IsActive { get; private set; }

        // Navigation property for refresh tokens belonging to this user
        public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
        public ICollection<UserEventRole> UserEventRoles { get; private set; }= new List<UserEventRole>();


        public void UpdateName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            SetUpdatedAt();
        }

        public void Deactivate()
        {
            IsActive = false;
            SetUpdatedAt();
        }

        public void Activate()
        {
            IsActive = true;
            SetUpdatedAt();
        }
    }
}
