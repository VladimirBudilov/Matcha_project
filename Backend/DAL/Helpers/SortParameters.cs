namespace DAL.Helpers;

public record SortParameters
{
    public string SortLocation { get; set; } = "ASC";
    public string SortFameRating { get; set; } = "DESC";
    public string SortAge { get; set; } = "DESC";
    public string SortCommonTags { get; set; } = "DESC";
    public int SortingMainParameter { get; set; } = 0;
    


    public List<string> ToList()
    {
        return new List<string> {SortLocation, SortFameRating, SortAge, SortCommonTags};
    }
}