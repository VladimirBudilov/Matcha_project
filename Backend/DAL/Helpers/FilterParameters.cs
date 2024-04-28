namespace DAL.Helpers;

public record FilterParameters
{
    public string? SexualPreferences { get; set; }
    public string? Location { get; set; }
    public List<string> CommonTags { get; set; } = new();
    public long? FameRating { get; set; }
}