namespace DAL.Entities;

public class Picture : AbstractEntity
{
    public int UserId { get; set; }
    public byte[]? PicturePath { get; set; }
    public bool IsProfilePicture { get; set; }
    
    public User User { get; set; }
}