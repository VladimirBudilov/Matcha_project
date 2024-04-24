﻿namespace Web_API.DTOs;

public record UserInfoRequestDto
{
    public string Username { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
}