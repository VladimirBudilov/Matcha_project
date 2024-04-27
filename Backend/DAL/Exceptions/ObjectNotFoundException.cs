using System.Runtime.Serialization;

namespace DAL.Helpers;

public class ObjectNotFoundException : Exception
{
    public ObjectNotFoundException(string message) : base(message)
    {
    }
    
    public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    public ObjectNotFoundException()
    {
    }
    
    public ObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}