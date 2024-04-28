using DAL.Entities;

namespace Web_API.DTOs;

public record ProfileFullDataForOtherUsersDto
{
    public long ProfileId { get; init; } = 0;
    public string UserName { get; init; } = "";
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Gender { get; init; } = "";
    public string SexualPreferences { get; init; } = "";
    public string Biography { get; init; } = "";
    public int FameRating { get; init; } = 0;
    public int Age { get; init; } = 0;
    public string Location { get; init; } = "";
    public string ProfilePicture { get; init; } = "";
    public List<string> Pictures { get; init; } = new();
    public List<string> Interests { get; init; } = new();
}