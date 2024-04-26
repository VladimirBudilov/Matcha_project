namespace BLL.Models;

public class ProfileModel
{
    public int ProfileId { get; set; }
    
    public string? Gender { get; set; }
    
    public string? SexualPreferences { get; set; }
    
    public string? Biography { get; set; }

    public string? ProfilePicture { get; set; }
    
    public int? FameRating { get; set; }
    
    public int? Age { get; set; }
    
    public string? Location { get; set; }

    public List<PicturesModel> Pictures { get; set; } = [];
    
    public List<LikesModel> Likes { get; set; } = [];
    
    public List<InterestsModel> Interests { get; set; } = [];

    
    
    
}