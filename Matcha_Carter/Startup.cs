using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System.Data.SQLite;
using CarterAndMVC.Database;

namespace CarterAndMVC
{
    using Carter;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCarter();
            services.AddControllers();
            services.AddSingleton<SQLiteDbService>(
            new SQLiteDbService(
                $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "../Matcha_Data/Matcha_db.sqlite")};Version=3;"));        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            app.UseRouting();
            app.UseEndpoints(builder =>
            {
                builder.MapDefaultControllerRoute();
                builder.MapCarter();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/api/data", async context =>
                {
                    var data = new { message = "Hello, Vue.js!" };
                    await context.Response.WriteAsJsonAsync(data);
                });
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/api/getdata", async context =>
                {
                    var dbService = context.RequestServices.GetRequiredService<SQLiteDbService>();
                    var data = dbService.GetData("SELECT * FROM User");
                    await context.Response.WriteAsJsonAsync(data);
                });
            });
            
            
        }
    }
}