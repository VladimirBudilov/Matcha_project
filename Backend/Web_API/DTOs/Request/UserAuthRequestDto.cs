namespace Web_API.DTOs.Request
{
    public record UserAuthRequestDto
    {
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
