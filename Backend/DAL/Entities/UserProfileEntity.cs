namespace DAL.Entities;

public class UserProfileEntity
{
    public int ProfileId { get; set; }
    
    public string Gender { get; set; }
    
    public string SexualPreferences { get; set; }
    
    public string Biography { get; set; }

    public int? ProfilePictureId { get; set; }
    
    public int FameRating { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}