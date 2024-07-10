namespace DAL.Entities;

public class Message : AbstractEntity
{
    public int RoomId { get; set; }
    public int SenderId { get; set; }
    public string Author { get; set; }
    public string Content { get; set; }
    public DateTime Created_at { get; set; }
}