using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Carter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;
using CarterAndMVC.Database;

var dbPath = Environment.GetEnvironmentVariable("DB_PATH");
var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
var frontUrl = Environment.GetEnvironmentVariable("FRONT_URL");
// print dbPath and baseUrl
Console.WriteLine($"DB_PATH: {dbPath}");
Console.WriteLine($"BASE_URL: {baseUrl}");
Console.WriteLine($"FRONT_URL: {frontUrl}");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseContentRoot(Directory.GetCurrentDirectory());
builder.Services.AddCarter();
builder.Services.AddControllers();
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

builder.Services.AddSingleton<SQLiteDbService>(
    new SQLiteDbService(
        $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), dbPath)}"));

var app = builder.Build();
app.UseDefaultFiles();
app.UseRouting();
app.MapControllers();
app.MapCarter();
app.UseCors();
app.Run();