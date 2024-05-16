using System.Text;
using DAL.Entities;
using Npgsql;

namespace DAL.Helpers;

public class ParameterInjector
{
    public void FillUserEntityParameters(User user, NpgsqlCommand command)
    {
        command.Parameters.AddWithValue("@userName", user.UserName);
        command.Parameters.AddWithValue("@firstName", user.FirstName);
        command.Parameters.AddWithValue("@lastName", user.LastName);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@password", user.Password);
        command.Parameters.AddWithValue("@isVerified", user.IsVerified ?? false);
    }

    public void InjectParameters(StringBuilder query, Dictionary<string, object> parameters)
    {
        foreach (var (key, value) in parameters)
        {
            query.Replace(key, value.ToString());
        }
    }
}