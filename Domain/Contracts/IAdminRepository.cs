namespace Domain.Contracts
{
    public interface IAdminRepository : IGenericRepository<Admin, Guid>
    {
        Task<Admin?> GetByEmailAsync(string email);
        string HashPassword(string password);
        bool VerifyPassword(Admin admin, string password);
    }
}
