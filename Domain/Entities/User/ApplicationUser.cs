namespace Domain.Entities.User
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? Name { get; set; }
        public string? Address { get; set; }

    }
}