namespace Web_API.DTOs;

public record MessageResponseDto
{
    public string Author { get; init; } 
    public string Content { get; init; }
    public DateTime Date { get; init; }
}