namespace Web_API.DTOs;

public record AuthResponseDto
{
    public bool Result { get; set; } = false;
    public string ErrorMessage { get; set; } = "";
    
    public string Token { get; set; } = "";
}