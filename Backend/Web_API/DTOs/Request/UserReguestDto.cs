namespace Web_API.DTOs
{
    public record UserReguestDto
    {
        public string UserName { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
    }
}
