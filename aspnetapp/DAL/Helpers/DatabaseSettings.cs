namespace DAL.Helpers;

public class DatabaseSettings(string? connectionString)
{
    public readonly string? ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
}