namespace Web_API.DTOs.Response;

public record ProfileResponse()
{
    public int ProfileId { get; init; } = 0;
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Gender { get; init; } = "";
    public string SexualPreferences { get; init; } = "";
    public int FameRating { get; init; } = 0;
    public int Age { get; init; } = 0;
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string ProfilePicture { get; init; } = "";
    public List<string> Interests { get; init; } = new();
    
    public bool HasLike { get; init; } = false;
    public double Distance { get; set; }
    
    public string LastSeen { get; set; } = "";
    public bool IsOnlineUser { get; set; } = false;
    public bool IsBlockedUSer { get; set; } = false;
};