namespace Web_API.DTOs.Response;

public record    FullProfileResponseDto
{
    public long ProfileId { get; init; }
    public string UserName { get; init; } = "";
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Gender { get; init; } = "";
    public string SexualPreferences { get; init; } = "";
    public string Biography { get; init; } = "";
    public int FameRating { get; init; } = 0;
    public int Age { get; init; } = 0;
    
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public PictureDto ProfilePicture { get; init; }
    public List<string> Interests { get; init; } = new();
    public List<PictureDto> Pictures { get; init; } = new();
    
    public bool HasLike { get; init; } = false;
    public double Distance { get; init; } = 0;
    
    public string LastLogin { get; set; } = "";
    public bool IsOnline { get; set; } = false;
    public string Email { get; set; } = "";
}