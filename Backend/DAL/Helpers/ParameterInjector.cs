using DAL.Entities;
using Microsoft.Data.Sqlite;

namespace DAL.Helpers;

public class ParameterInjector
{
    public void FillUserEntityParameters(UserEntity user, SqliteCommand command)
    {
        command.Parameters.AddWithValue("@userName", user.UserName);
        command.Parameters.AddWithValue("@firstName", user.FirstName);
        command.Parameters.AddWithValue("@lastName", user.LastName);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@password", user.Password);
        command.Parameters.AddWithValue("@updatedAt", user.UpdatedAt ?? DateTime.Now);
        command.Parameters.AddWithValue("@createdAt", user.CreatedAt ?? DateTime.Now);
        command.Parameters.AddWithValue("@lastLoginAt", user.LastLoginAt ?? DateTime.Now);
        command.Parameters.AddWithValue("@resetTokenExpiry", user.ResetTokenExpiry ?? DateTime.Now);
        command.Parameters.AddWithValue("@resetToken", user.ResetToken ?? "");
        command.Parameters.AddWithValue("@isVerified", user.IsVerified ?? false);
    }
}