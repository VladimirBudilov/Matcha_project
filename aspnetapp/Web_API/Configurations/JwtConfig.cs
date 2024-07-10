namespace Web_API.Configurations;

public class JwtConfig
{
    public string Secret { get; set; } = Environment.GetEnvironmentVariable("JwtConfig__Secret");
}