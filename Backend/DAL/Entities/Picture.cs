namespace DAL.Entities;

public class Picture
{
    public long PictureId { get; set; }
    public long UserId { get; set; }
    public byte[]? PicturePath { get; set; }
    public bool IsProfilePicture { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public User User { get; set; }
}