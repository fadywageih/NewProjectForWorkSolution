using NewProjectForWork.Extensions;
using NewProjectForWork.Extentions;
using System.Text.Json;

namespace NewProjectForWork
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Services
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            builder.Services.AddPressentionServices();
            builder.Services.AddCoreServices(builder.Configuration);
            builder.Services.AddInfrasturctureServices(builder.Configuration);
            #endregion

            var app = builder.Build();
            app.UseCustomMiddleWare();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}