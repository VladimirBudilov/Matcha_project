using System.Runtime.Serialization;

namespace Web_API.Helpers;

public class DataValidationException : Exception
{
    public DataValidationException(string message) : base(message)
    {
    }
    
    public DataValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    public DataValidationException()
    {
    }
    
    public DataValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}