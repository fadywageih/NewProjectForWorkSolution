using NewProjectForWork.Extensions;
using NewProjectForWork.Extentions;
using System.Text.Json;

namespace NewProjectForWork
{
    public class Program
    {
        public static async Task Main(string[] args)  // ✅ 1. تغيير void إلى async Task
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Services
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            builder.Services.AddHttpContextAccessor();

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

            // ✅ 2. استدعاء Seed قبل تشغيل التطبيق
            await app.SeedDatabaseAsync();  // <-- هذا السطر كان ناقص!

            await app.RunAsync();  // ✅ 3. استخدم RunAsync بدل Run
        }
    }
}