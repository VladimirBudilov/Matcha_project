namespace DAL.Helpers;

public record SortingParameters
{
    public long Age { get; set; }
    public long FameRating { get; set; }
    public string Location { get; set; }
    public List<string> CommonTags { get; set; }
}