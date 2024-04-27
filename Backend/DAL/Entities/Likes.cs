namespace DAL.Entities;

public class Likes
{
    public int LikerUserId { get; set; }
    public int LikedUserId { get; set; }
    public DateTime? LakedAt { get; set; }
    
    public User LikerUser { get; set; }
    public User LikedUser { get; set; }
}