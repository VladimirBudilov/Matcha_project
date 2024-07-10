using System.Runtime.Serialization;

namespace Web_API.Helpers;

public class NotAuthorizedRequestException : Exception
{
    public NotAuthorizedRequestException(string message) : base(message)
    {
    }
    
    public NotAuthorizedRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    public NotAuthorizedRequestException()
    {
    }
    
    public NotAuthorizedRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}