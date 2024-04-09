using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Carter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;
using CarterAndMVC.Database;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Http;

var dbPath = Environment.GetEnvironmentVariable("DB_PATH");
var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
var frontUrl = Environment.GetEnvironmentVariable("FRONT_URL");
// print dbPath and baseUrl
Console.WriteLine($"DB_PATH: {dbPath}");
Console.WriteLine($"BASE_URL: {baseUrl}");
Console.WriteLine($"FRONT_URL: {frontUrl}");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Host.UseContentRoot(Directory.GetCurrentDirectory());
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.WebHost.UseUrls(baseUrl);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureHttpsDefaults(co =>
    {
        co.ClientCertificateMode = ClientCertificateMode.NoCertificate;
        //co.ServerCertificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "cert/aspn.pfx"), "crypticpassword");
    });
});

//add swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);
});



builder.Services.AddSingleton<SQLiteDbService>(
    new SQLiteDbService(
        $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), dbPath)}"));

var app = builder.Build();

//add swagger UI

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});

app.UseDefaultFiles();
app.UseRouting();
app.MapControllers();
//app.MapCarter();
app.UseCors();

app.MapGet("/user/{id}", (Guid id) => Results.Ok($"get user with id {id}"));
app.MapGet("/user", () => Results.Ok("get all users"));
app.MapPost("/user", () => Results.Ok("create user"));
app.MapPut("/user/{id}", (Guid id) => Results.Ok($"update user with id {id}"));
app.MapDelete("/user/{id}", (Guid id) => Results.Ok($"delete user with id {id}"));


app.Run();