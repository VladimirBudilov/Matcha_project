﻿namespace Web_API.DTOs
{
    public record UserDto
    {
        public string UserName { get; init; } = "";
        public string FirstName { get; init; } = "";
        public string LastName { get; init; } = "";
        public string Email { get; init; } = "";
    }
}
