using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using CarterAndMVC.Database;
using Carter;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseContentRoot(Directory.GetCurrentDirectory());

builder.Services.AddCarter();
builder.Services.AddControllers();
builder.Services.AddSingleton<SQLiteDbService>(
    new SQLiteDbService(
        $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "../Matcha_Data/Matcha_db.sqlite")};Version=3;"));

var app = builder.Build();

app.UseDefaultFiles();
app.UseRouting();

app.MapControllers();
app.MapCarter();

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

app.Run();