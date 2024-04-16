namespace DAL.Entities;

public class ProfileViewsEntity
{
    public int ViewerUserId { get; set; }
    public int ViewedUserId { get; set; }
    public DateTime ViewedAt { get; set; }
}