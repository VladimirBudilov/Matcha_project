namespace DAL.Helpers;

public class Parameters
{
    public SortParameters Sort { get; set; } = new();
    public SearchParameters Search { get; set; } = new();
    public PaginationParameters Pagination { get; set; } = new();
}