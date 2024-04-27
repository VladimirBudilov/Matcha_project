namespace DAL.Entities;

public class Pictures
{
    public int PictureId { get; set; }
    public int? UserId { get; set; }
    public string? ImagePath { get; set; }
    public bool IsProfilePicture { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public User User { get; set; }
}