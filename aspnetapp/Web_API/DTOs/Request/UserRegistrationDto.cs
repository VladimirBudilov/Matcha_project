﻿namespace Web_API.DTOs.Request;

public record UserRegistrationDto
{
    public string UserName { get; init; } = "";
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
}