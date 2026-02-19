namespace Shared.Dtos
{
    public class AdminResultDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
