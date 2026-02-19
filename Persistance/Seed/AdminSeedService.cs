namespace Persistance.Seed
{
    public interface IAdminSeedService
    {
        Task SeedSuperAdminAsync();
    }

    public class AdminSeedService : IAdminSeedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AdminSeedService> _logger;

        public AdminSeedService(IServiceProvider serviceProvider, ILogger<AdminSeedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task SeedSuperAdminAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var adminRepository = scope.ServiceProvider.GetRequiredService<IAdminRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var superAdminEmail = "super@admin.com";
            var superAdminPassword = "Kingfady@123123";

            // Check if already exists
            var existingAdmin = await adminRepository.GetByEmailAsync(superAdminEmail);

            if (existingAdmin != null)
            {
                _logger.LogInformation("Super Admin already exists. Updating password...");

                // ✅ Update existing admin
                existingAdmin.PasswordHash = adminRepository.HashPassword(superAdminPassword);
                existingAdmin.Role = "SuperAdmin";
                existingAdmin.IsActive = true;

                adminRepository.Update(existingAdmin);
                await unitOfWork.SaveChangesAsync();

                _logger.LogInformation("✅ Super Admin password updated successfully!");
                return;
            }

            // Create Super Admin
            var superAdmin = new Admin
            {
                Id = Guid.NewGuid(),
                Email = superAdminEmail,
                FirstName = "Super",
                LastName = "Admin",
                PasswordHash = adminRepository.HashPassword(superAdminPassword),
                Role = "SuperAdmin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await adminRepository.AddAsync(superAdmin);
            await unitOfWork.SaveChangesAsync();

            _logger.LogInformation("✅ Super Admin created successfully!");
        }
    }
}
