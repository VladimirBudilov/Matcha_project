using System.Runtime.Serialization;

namespace DAL.Helpers;

public class DataAccessErrorException : Exception
{
    public DataAccessErrorException(string message) : base(message)
    {
    }
    
    public DataAccessErrorException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    public DataAccessErrorException()
    {
    }
    
    public DataAccessErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}