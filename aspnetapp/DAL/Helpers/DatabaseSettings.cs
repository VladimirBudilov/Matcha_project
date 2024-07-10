using Microsoft.Extensions.Configuration;

namespace DAL.Helpers;

public class DatabaseSettings(IConfiguration configuration)
{
    public readonly string ConnectionString = Environment.GetEnvironmentVariable("DatabaseSettings__ConnectionString");
}