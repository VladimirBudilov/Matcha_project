namespace DAL.Helpers;

public record SearchParameters
{
    public string? SexualPreferences { get; set; } = null;
    public int? MaxDistance { get; set; } = null;
    public int? MinFameRating { get; set; } = null;
    public int? MaxFameRating { get; set; } = null;
    public int? MaxAge { get; set; } = null;
    public int? MinAge { get; set; } = null;
    public bool? IsLikedUser { get; set; } = null;
    public List<string> CommonTags { get; set; } = new();
    public bool? IsMatched { get; set; } = null;
}