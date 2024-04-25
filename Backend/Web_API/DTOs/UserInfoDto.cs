﻿namespace Web_API.DTOs;

public record UserInfoDto
{
    public string Username { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Gender { get; init; }
    public string SexualPreferences { get; init; }
    public string Biography { get; init; }
    public int FameRating { get; init; }
    public int Age { get; init; }
    public string Location { get; init; }
    public string ProfilePicture { get; init; }
    public List<string> Pictures { get; init; }
}