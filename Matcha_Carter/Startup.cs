using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

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
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            /*app.UseStaticFiles(
                new StaticFileOptions
                {
                    //TODO - Change this to your own path. 
                    FileProvider = new PhysicalFileProvider(
                        Directory.GetCurrentDirectory()+"\\wwwroot"),
                    RequestPath = ""
                });*/
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
        }
    }
}