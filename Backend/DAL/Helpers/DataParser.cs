using System.Data;

namespace DAL.Helpers;

public class DataParser
{
    public DateTime? TryParseDateTime(string value)
    {
        if (DateTime.TryParse(value, out var result))
        {
            return result;
        }

        return null;
    }
        
    public  T? GetClassValueOrNull<T>(DataRow row, string columnName) where T : class
    {
        return row.IsNull(columnName) ? null : row.Field<T>(columnName);
    }

    public  T? GetStructValueOrNull<T>(DataRow row, string columnName) where T : struct
    {
        return row.IsNull(columnName) ? (T?)null : row.Field<T>(columnName);
    }
        
    public bool? ConvertLongToBool(long? value)
    {
        if (value == null)
        {
            return null;
        }

        return value != 0;
    }
        
    public T? TryParse<T>(string value) where T : struct
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
        if (converter != null && converter.IsValid(value))
        {
            return (T)converter.ConvertFromString(value);
        }
        return null;
    }
}