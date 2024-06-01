using System.Runtime.Serialization;

namespace Web_API.Helpers;

public class ForbiddenActionException : Exception
{
    public ForbiddenActionException(string message) : base(message)
    {
    }
    
    public ForbiddenActionException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    public ForbiddenActionException()
    {
    }
    
    public ForbiddenActionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}