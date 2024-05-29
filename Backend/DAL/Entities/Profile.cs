namespace DAL.Entities;

public class Profile : AbstractEntity
{
    public string Gender { get; set; }
    public string? SexualPreferences { get; set; }
    public string? Biography { get; set; }
    public int? ProfilePictureId { get; set; }
    public int? FameRating { get; set; }
    public int Age { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsActive { get; set; }
  
    public Picture? ProfilePicture { get; set; }
    public List<Picture> Pictures { get; set; } = new List<Picture>();
    public List<Interest> Interests { get; set; } = new List<Interest>();
    public List<Like> Likes { get; set; } = new List<Like>();

    public bool HasEmptyFields()
    {
        return SexualPreferences is null || Biography is null || Latitude is null || Longitude is null; 
    }
}
