namespace Web_API.DTOs;

public class ResponseDto<T>
{
    public T Data { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}