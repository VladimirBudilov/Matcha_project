namespace DAL.Helpers;

public record SearchParameters
{
    public string? SexualPreferences { get; set; } = null;
    public long? MaxDistance { get; set; } = null;
    public long? MinFameRating { get; set; } = null;
    public long? MaxFameRating { get; set; } = null;
    public long? MaxAge { get; set; } = null;
    public long? MinAge { get; set; } = null;
    public List<string> CommonTags { get; set; } = new();
}