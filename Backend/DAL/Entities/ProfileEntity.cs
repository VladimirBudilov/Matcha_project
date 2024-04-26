﻿namespace DAL.Entities;

public class ProfileEntity
{
    public long? ProfileId { get; set; }
    
    public string Gender { get; set; }
    
    public string? SexualPreferences { get; set; }
    
    public string? Biography { get; set; }

    public long? ProfilePictureId { get; set; }
    
    public long? FameRating { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public string? Location { get; set; }
    
    public List<string> Interests { get; set; } = new List<string>();
    
    public string? ProfilePicture { get; set; }
    
    public List<string> Pictures { get; set; } = new List<string>();
    
    public long ViewsAmount { get; set; }
    
    public long LikesAmount { get; set; }
    public long Age { get; set; }
}