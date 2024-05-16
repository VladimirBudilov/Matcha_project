namespace DAL.Helpers;

public record SearchParameters
{
    public int UserId { get; set; }
    public string? SexualPreferences { get; set; } = null;
    public long? MaxDistance { get; set; } = null;
    public long? MinFameRating { get; set; } = null;
    public long? MaxFameRating { get; set; } = null;
    public long? MaxAge { get; set; } = null;
    public long? MinAge { get; set; } = null;
    public bool? IsLikedUser { get; set; } = null;
    public List<string> CommonTags { get; set; } = new();
    public bool? IsMatched { get; set; } = null;
}