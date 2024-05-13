namespace DAL.Helpers;

public record SortParameters
{
    public string? Location { get; set; } = "ASC";
    public string? FameRating { get; set; } = "DESC";
    public string? Age { get; set; } = "DESC";
    public string? CommonTags { get; set; } = "DESC";
    public int? MainParameter { get; set; }
}