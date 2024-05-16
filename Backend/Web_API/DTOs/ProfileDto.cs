namespace Web_API.DTOs;

public record ProfileDto
{
    public string Gender { get; init; } = "";
    public string SexualPreferences { get; init; } = "";
    public string Biography { get; init; } = "";
    public int Age { get; init; } = 0;
    
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public List<string> Interests { get; init; } = new();
}