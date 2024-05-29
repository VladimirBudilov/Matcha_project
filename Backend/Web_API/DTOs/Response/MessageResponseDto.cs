namespace Web_API.DTOs;

public record MessageResponseDto
{
    public int Id { get; init; }
    public string Photo { get; init; } 
    public string Text { get; init; }
    public DateTime Date { get; init; }
}