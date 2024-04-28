namespace Web_API.DTOs;

public class ProfileUpdateRequest
{
    public string Gender { get; init; } = "";
    public string SexualPreferences { get; init; } = "";
    public string Biography { get; init; } = "";
    public int Age { get; init; } = 0;
    public string Location { get; init; } = "";
}