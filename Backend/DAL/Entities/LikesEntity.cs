namespace DAL.Entities;

public class LikesEntity
{
    public int LikerUserId { get; set; }
    public int LikedUserId { get; set; }
    public DateTime? LakedAt { get; set; }
}