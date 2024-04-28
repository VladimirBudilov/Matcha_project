namespace DAL.Entities;

public class Like
{
    public long LikerId { get; set; }
    public long LikedId { get; set; }
    
    public DateTime? LakedAt { get; set; }
    public string LikerUserName { get; set; }

}