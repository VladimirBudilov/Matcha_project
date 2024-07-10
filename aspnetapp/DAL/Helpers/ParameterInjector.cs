using System.Text;
using DAL.Entities;
using Npgsql;

namespace DAL.Helpers;

public class ParameterInjector
{
    public void InjectParameters(StringBuilder query, Dictionary<string, object> parameters)
    {
        foreach (var (key, value) in parameters)
        {
            var valueStr = value is string ? $"'{value}'" : value.ToString();
            query.Replace(key, valueStr);
        }
    }
}