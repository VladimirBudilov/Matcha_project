namespace DAL.Helpers;

public record SortParameters
{
    public string? Age { get; set; } = "DESC";
    public string? FameRating { get; set; } = "DESC";
    public string? Location { get; set; } = "ASC";
    public string? CommonTags { get; set; } = "DESC";
    public int? MainParameter { get; set; }
}