namespace DAL.Entities;

public class ProfileViews
{
    public int ViewerUserId { get; set; }
    public int ViewedUserId { get; set; }
    public DateTime? ViewedAt { get; set; }
    
    public User ViewerUser { get; set; }
    public User ViewedUser { get; set; }
}