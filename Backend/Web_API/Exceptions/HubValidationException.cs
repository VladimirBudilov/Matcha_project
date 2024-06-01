namespace Web_API.Helpers;

public class HubValidationException : Exception
{
    public HubValidationException(string message) : base(message)
    {
    }
    
    public HubValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    public HubValidationException()
    {
    }
}