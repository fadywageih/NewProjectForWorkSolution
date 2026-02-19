using NewProjectForWork.MiddleWares;
using Persistance.Seed;

namespace NewProjectForWork.Extentions
{
    public static class WebApplicationExtenstions
    {
        public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            // Seed Super Admin
            var seedService = scope.ServiceProvider.GetRequiredService<IAdminSeedService>();
            await seedService.SeedSuperAdminAsync();

            return app;
        }

        public static WebApplication UseCustomMiddleWare(this WebApplication app)
        {
            app.UseMiddleware<GlobalErrorHandlingMiddleWare>();
            return app;
        }

    }
}
