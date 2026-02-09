using Services; 
using ServicesAbstraction;
using Shared;

namespace NewProjectForWork.Extensions
{
    public static class CoreServicesExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IServiceManager, ServiceManager>();
            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            services.AddAutoMapper(typeof(Services.AssemblyReference).Assembly);

            return services;
        }
    }
}