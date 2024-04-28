namespace Web_API.DTOs;

public record ProfilePIctureDto
{
    public long ProfileId { get; init; }
    public string ProfilePicture { get; init; }
}