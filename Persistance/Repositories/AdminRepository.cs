namespace Persistance.Repositories
{
    public class AdminRepository : GenericRepository<Admin, Guid>, IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _context.Set<Admin>()
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(Admin admin, string password)
        {
            return admin.PasswordHash == HashPassword(password);
        }
    }
}
