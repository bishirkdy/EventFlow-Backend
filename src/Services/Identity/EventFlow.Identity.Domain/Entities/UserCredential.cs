namespace EventFlow.Identity.Domain.Entities
{
    public class UserCredential : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string PasswordHash { get; private set; } = string.Empty;

        private UserCredential() { }

        public UserCredential(Guid userId , string passwordHadh)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            PasswordHash = passwordHadh;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdatePassword(string passwordHash)
        {
            PasswordHash = passwordHash;
            UpdatedAt = UpdatedAt;
        }
    }
}
