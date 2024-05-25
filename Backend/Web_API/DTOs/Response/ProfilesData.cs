namespace Web_API.DTOs;

public class ProfilesData
{
    public List<ProfileResponse> Profiles { get; set; }
    public long AmountOfPages { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}