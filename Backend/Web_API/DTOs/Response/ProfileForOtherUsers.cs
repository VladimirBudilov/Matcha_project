namespace Web_API.DTOs;

public record ProfileForOtherUsers()
{
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Gender { get; init; } = "";
    public string SexualPreferences { get; init; } = "";
    public int FameRating { get; init; } = 0;
    public int Age { get; init; } = 0;
    public string ProfilePicture { get; init; } = "";
    public List<string> Interests { get; init; } = new();
    
};