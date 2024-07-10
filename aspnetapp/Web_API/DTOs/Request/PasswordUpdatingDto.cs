namespace Web_API.DTOs.Request;

public record PasswordUpdatingDto
{
    public string OldPassword { get; init; } = "";
    public string NewPassword { get; init; } = "";
}