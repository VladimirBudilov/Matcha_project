namespace DAL.Entities;

public class Like
{
    public int LikerId { get; set; }
    public int LikedId { get; set; }
    
    public string LikerUserName { get; set; }

}