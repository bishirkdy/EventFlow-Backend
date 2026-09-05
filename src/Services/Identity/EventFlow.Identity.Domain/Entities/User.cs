namespace EventFlow.Identity.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; private set; } = string.Empty;
        public string UserName { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }
        private User(){}

        public User(string email,string userName,string firstName,string lastName)
        {
            Id = Guid.NewGuid();

            Email = email;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;

            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
