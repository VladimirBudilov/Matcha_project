namespace Web_API.DTOs;

public record PasswordUpdatingDto
{
    public string OldPassword { get; init; } = "";
    public string NewPassword { get; init; } = "";
}