using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NewProjectForWork.Factories;

namespace NewProjectForWork.Extentions
{
    public static class PressentionServicesExsention
    {
        public static IServiceCollection AddPressentionServices(this IServiceCollection Services)
        {
            Services.AddControllers().AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);
            Services.AddEndpointsApiExplorer();
            Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
            Services.AddCors(options => {
                options.AddPolicy("CorsPolicy", bulider =>
                {
                    bulider
          .WithOrigins("http://localhost:4200", "https://localhost:4200")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
                });
            });
            Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiResponseFactory.CustomValidationErrorResponse;
            });
            return Services;
        }
    }
}
