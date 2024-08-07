﻿namespace Web_API.DTOs.Response;

public record AuthResponseDto
{
    public bool Result { get; set; } = false;
    public string Error { get; set; } = "";
    public string Token { get; set; } = "";
}