using System;
using Carter;
using CarterAndMVC.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CarterAndMVC;

public class UserModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/data", async context =>
        {
            var data = new { message = "Hello, Vue.js!" };
            await context.Response.WriteAsJsonAsync(data);
        });

        app.MapGet("/api/getdata", async context =>
        {
            var dbService = context.RequestServices.GetRequiredService<SQLiteDbService>();
            var data = dbService.GetData("SELECT * FROM User");
            await context.Response.WriteAsJsonAsync(data);
        });
        
        app.MapPost("/api/signup", async context =>
        {
            var email = context.Request.Body;
            Console.WriteLine(email);
        });
    }

}