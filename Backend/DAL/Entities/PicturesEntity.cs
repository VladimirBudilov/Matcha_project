namespace DAL.Entities;

public class PicturesEntity
{
    public int PictureId { get; set; }
    public int? UserId { get; set; }
    public string? ImagePath { get; set; }
    public bool IsProfilePicture { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}