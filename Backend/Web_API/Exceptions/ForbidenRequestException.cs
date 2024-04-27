using System.Runtime.Serialization;

namespace Web_API.Helpers;

public class ForbiddenRequestException : Exception
{
    public ForbiddenRequestException(string message) : base(message)
    {
    }
    
    public ForbiddenRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    public ForbiddenRequestException()
    {
    }
    
    public ForbiddenRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}