namespace Web_API.DTOs;

public record ProfileCreationRequestDto
{
    public string Gender { get; init; } = "";
    public string SexualPreferences { get; init; } = "";
    public string Biography { get; init; } = "";
    public int Age { get; init; } = 0;
    public string Location { get; init; } = "";
    public string ProfilePicture { get; init; } = "";
    public List<string> Pictures { get; init; } = new();
}