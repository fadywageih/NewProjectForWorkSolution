namespace Domain.Contracts
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(Guid id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser> AddAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
    }
}
