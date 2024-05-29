namespace DAL.Entities;

public class Room : AbstractEntity
{
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; }
    public DateTime Created_at { get; set; }
}