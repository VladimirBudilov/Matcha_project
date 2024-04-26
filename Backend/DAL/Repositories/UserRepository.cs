using System.Data;
using DAL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;

namespace DAL.Repositories
{
    public class UserRepository(IConfiguration configuration)
    {
        private readonly string _connectionString = configuration.GetConnectionString("UserDbConnection")
                                                    ?? throw new ArgumentNullException(nameof(configuration),
                                                        "Connection string not found in configuration");
        #region GettingData

        public async Task<UserEntity?> GetUserByIdAsync(int id)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await GetTableByParameter(connection, "SELECT * FROM users WHERE user_id = @id", "@id", id);
            return dataTable.Rows.Count > 0 ? CreateUser(dataTable.Rows[0]) : null;
        }

        public async Task<UserEntity> GetUserByEmailAsync(string email)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var dataTable = await GetTableByParameter(connection, "SELECT * FROM users WHERE email = @email", "@email", email);
                return dataTable.Rows.Count > 0 ? CreateUser(dataTable.Rows[0]) : null;
            }
        }

        public async Task<UserEntity?> GetUserByUserNameAsync(string userName)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await GetTableByParameter(connection, "SELECT * FROM users WHERE user_name = @userName", "@userName", userName);
            return dataTable.Rows.Count > 0 ? CreateUser(dataTable.Rows[0]) : null;
        }

        public async Task<IEnumerable<UserEntity>> GetAllUsers()
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await GetTableByParameter(connection, "SELECT * FROM users", null, null);
            var users = new List<UserEntity>();
            foreach (DataRow row in dataTable.Rows)
            {
                users.Add(CreateUser(row));
            }

            return users;
        }
        #endregion

        #region GettingFullData

        public async Task<UserEntity?> GetFullDataAsync(int id)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            var userInfo = await GetTableByParameter(connection, "SELECT * FROM users" +
                                                             " LEFT JOIN profiles ON users.user_id = profiles.profile_id" +
                                                             " WHERE user_id = @id", "@id", id);
            var userInterestsById = await GetTableByParameter(connection, "SELECT * FROM user_interests WHERE user_id = @id",
                "@id", id);
            var interestsIds = (from DataRow row in userInterestsById.Rows select row.Field<long>("interest_id")).ToList();
            command.CommandText = "SELECT * FROM interests WHERE interest_id IN (" +
                                  string.Join(",", interestsIds) + ")";
            var interests = new DataTable();
            interests.Load(await command.ExecuteReaderAsync());
            var likes = await GetTableByParameter(connection, "SELECT * FROM likes WHERE liked_user_id = @id", "@id",
                id);
            var pictures =
                await GetTableByParameter(connection, "SELECT * FROM pictures WHERE user_id = @id", "@id", id);
            var profileViews = await GetTableByParameter(connection,
                "SELECT * FROM profile_views WHERE profile_user_id = @id", "@id", id);
            var userInterests = (from DataRow row in interests.Rows select row.Field<string>("name")).ToList();
            var likesAmount = likes.Rows.Count;
            var profilePictures = new List<string>();
            var mainProfilePicture = string.Empty;
            foreach (DataRow row in pictures.Rows)
            {
                if (row.Field<long>("is_profile_picture") == 0)
                {
                    mainProfilePicture = row.Field<string>("image_path");
                    continue;
                }

                profilePictures.Add(row.Field<string>("image_path"));
            }
            var profileViewsAmount = profileViews.Rows.Count;
            if (userInfo.Rows.Count > 0)
            {
                var userInfoRow = userInfo.Rows[0];
                var user = CreateUser(userInfoRow);
                user.Profile = CreateUserProfile(userInfoRow, mainProfilePicture, userInterests, profilePictures, profileViewsAmount, likesAmount);
                return user;
            }
            return null;
        }

        private static ProfileEntity CreateUserProfile(DataRow userInfoRow, string? mainProfilePicture, List<string> userInterests, List<string> profilePictures, int profileViewsAmount, int likesAmount)
        {
            return new ProfileEntity
            {
                ProfileId = userInfoRow.IsNull("profile_id") ? null : userInfoRow.Field<long>("profile_id"),
                Gender = userInfoRow.IsNull("gender") ? null : userInfoRow.Field<string>("gender"),
                SexualPreferences = userInfoRow.IsNull("sexual_preferences") ? null : userInfoRow.Field<string>("sexual_preferences"),
                Biography = userInfoRow.IsNull("biography") ? null : userInfoRow.Field<string>("biography"),
                ProfilePictureId = userInfoRow.IsNull("profile_picture_id") ? (long?)null : userInfoRow.Field<long>("profile_picture_id"),
                FameRating = userInfoRow.IsNull("fame_rating") ? null : userInfoRow.Field<long>("fame_rating"),
                Location = userInfoRow.IsNull("location") ? null : userInfoRow.Field<string>("location"),
                Age = userInfoRow.Field<long>("age"),
                ProfilePicture = mainProfilePicture, 
                Interests = userInterests,
                Pictures = profilePictures,
                ViewsAmount = profileViewsAmount,
                LikesAmount = likesAmount,
            };
        }

        private UserEntity? CreateUser(DataRow row)
        {
            return new UserEntity
            {
                UserId = row.IsNull("user_id") ? (long?)null : row.Field<long>("user_id"),
                IsVerified = row.IsNull("is_verified") ? (bool?)null : ConvertLongToBool(row.Field<long>("is_verified")),
                UserName = row.Field<string>("user_name"),
                FirstName = row.Field<string>("first_name"),
                LastName = row.Field<string>("last_name"),
                Email = row.Field<string>("email"),
                Password = row.Field<string>("password"),
                UpdatedAt = TryParseDateTime(row.Field<string>("updated_at")),
                CreatedAt = TryParseDateTime(row.Field<string>("created_at")),
                LastLoginAt = TryParseDateTime(row.Field<string>("last_login_at")),
                ResetTokenExpiry = TryParseDateTime(row.Field<string>("reset_token_expiry")),
                ResetToken = row.Field<string?>("reset_token"),
            };
        }
        
        private DateTime? TryParseDateTime(string value)
        {
            if (DateTime.TryParse(value, out var result))
            {
                return result;
            }

            return null;
        }
        
        private static T? GetClassValueOrNull<T>(DataRow row, string columnName) where T : class
        {
            return row.IsNull(columnName) ? null : row.Field<T>(columnName);
        }

        private static T? GetStructValueOrNull<T>(DataRow row, string columnName) where T : struct
        {
            return row.IsNull(columnName) ? (T?)null : row.Field<T>(columnName);
        }
        
        private bool? ConvertLongToBool(long? value)
        {
            if (value == null)
            {
                return null;
            }

            return value != 0;
        }
        
        private T? TryParse<T>(string value) where T : struct
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

        private static async Task<DataTable> GetTableByParameter(SqliteConnection connection, string sqlQuery,
            string parameterName,
            int parameter)
        {
            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            command.Parameters.AddWithValue(parameterName, parameter);
            var readerAsync = await command.ExecuteReaderAsync();
            var table = new DataTable();
            if (readerAsync.HasRows)
            {
                table.Load(readerAsync);
            }
            return table;
        }
        
        private static async Task<DataTable> GetTableByParameter(SqliteConnection connection, string sqlQuery,
            string parameterName,
            string parameter)
        {
            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            command.Parameters.AddWithValue(parameterName, parameter);
            var readerAsync = await command.ExecuteReaderAsync();
            var table = new DataTable();
            if (readerAsync.HasRows)
            {
                table.Load(readerAsync);
            }
            return table;
        }

        #endregion

        public async Task<UserEntity?> AddUserAsync(UserEntity user)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO users (user_name, first_name, last_name, email, password, updated_at, created_at, last_login_at, reset_token_expiry, reset_token, is_verified)" +
                "VALUES (@userName, @firstName, @lastName, @email, @password, @updatedAt, @createdAt, @lastLoginAt, @resetTokenExpiry, @resetToken, @isVerified)";
            FillUserEntityParameters(user, command);
            await command.ExecuteNonQueryAsync();
            return user;
        }

        public async Task<UserEntity?> UpdateUserAsync(int id, UserEntity user)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText =
                "UPDATE users SET user_name = @userName, first_name = @firstName, last_name = @lastName, email = @email, password = @password, " +
                "updated_at = @updatedAt, created_at = @createdAt, last_login_at = @lastLoginAt, reset_token_expiry = @resetTokenExpiry, reset_token = @resetToken, is_verified = @isVerified " +
                "WHERE user_id = @id";
            FillUserEntityParameters(user, command);
            command.Parameters.AddWithValue("@id", id);
            await command.ExecuteNonQueryAsync();
            return user;
        }

        public async Task DeleteUserAsync(int id)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM User WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                await command.ExecuteNonQueryAsync();
            }
        }


        #region Utils

        private static void FillUserEntityParameters(UserEntity user, SqliteCommand command)
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

        #endregion
    }
}