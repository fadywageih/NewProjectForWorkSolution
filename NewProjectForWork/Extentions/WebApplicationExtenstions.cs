using NewProjectForWork.MiddleWares;

namespace NewProjectForWork.Extentions
{
    public static class WebApplicationExtenstions
    {
        public static async Task<WebApplication> SeedDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            return app;
        }
        public static WebApplication UseCustomMiddleWare(this WebApplication app)
        {
            app.UseMiddleware<GlobalErrorHandlingMiddleWare>();
            return app;
        }

    }
}
