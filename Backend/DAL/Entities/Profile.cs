namespace DAL.Entities;

public class Profile
{
    public long ProfileId { get; set; }
    public string Gender { get; set; }
    public string? SexualPreferences { get; set; }
    public string? Biography { get; set; }
    public long? ProfilePictureId { get; set; }
    public long? FameRating { get; set; }
    public long Age { get; set; }
    public long? Location { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    
    public Picture? ProfilePicture { get; set; }
    public List<Picture> Pictures { get; set; } = new List<Picture>();
    public List<Interests> Interests { get; set; } = new List<Interests>();
    public List<Like> Likes { get; set; } = new List<Like>();
    
}
