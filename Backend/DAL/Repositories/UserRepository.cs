using System.Data;
using DAL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;

namespace DAL.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("UserDbConnection")
                                ?? throw new ArgumentNullException(nameof(configuration),
                                    "Connection string not found in configuration");
        }

        #region GettingData

        public async Task<UserEntity?> GetUserByIdAsync(int id)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM users WHERE user_id = @id";
            command.Parameters.AddWithValue("@id", id);
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return CreateUserEntity(reader);
            }

            return null;
        }

        public async Task<IEnumerable<UserEntity>> GetAllUsers()
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM users";
            var reader = await command.ExecuteReaderAsync();
            var users = new List<UserEntity>();
            while (await reader.ReadAsync())
            {
                users.Add(CreateUserEntity(reader));
            }

            return users;
        }

        public async Task<UserEntity> GetUserByEmailAsync(string email)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM users WHERE email = @email";
                command.Parameters.AddWithValue("@email", email);
                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return CreateUserEntity(reader);
                }

                return null;
            }
        }

        public async Task<UserEntity?> GetUserByUserNameAsync(string userName)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM users WHERE user_name = @userName";
            command.Parameters.AddWithValue("@userName", userName);
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return CreateUserEntity(reader);
            }

            return null;
        }

        #endregion

        #region GettingFullData

        public async Task<UserEntity?> GetFullDataAsync(int id)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            //collect data about user and profile
            var userInfo = await GetTableByParameter(connection, "SELECT * FROM users" +
                                                             " LEFT JOIN profiles ON users.user_id = profiles.user_id" +
                                                             " WHERE user_id = @id", "@id", id);
            // collect data about user interests
            var intersts = await GetTableByParameter(connection, "SELECT * FROM user_interests WHERE user_id = @id",
                "@id", id);
            //collect data about howmach peopel liked his prfile by liked_user_id
            var likes = await GetTableByParameter(connection, "SELECT * FROM likes WHERE liked_user_id = @id", "@id",
                id);
            //collect all users pictures by user_id
            var pictures =
                await GetTableByParameter(connection, "SELECT * FROM pictures WHERE user_id = @id", "@id", id);
            //collect data about profile views
            var profileViews = await GetTableByParameter(connection,
                "SELECT * FROM profile_views WHERE viewed_user_id = @id", "@id", id);
            //get all user interests by their name
            var userInterests = new List<string>();
            foreach (DataRow row in intersts.Rows)
            {
                userInterests.Add(row.Field<string>("interest"));
            }

            //get amount of likes
            var likesAmount = likes.Rows.Count;
            //get user pictures
            var profilePictures = new List<string>();
            string? mainProfilePicture = string.Empty;
            foreach (DataRow row in pictures.Rows)
            {
                if (row.Field<bool>("is_profile_picture"))
                {
                    mainProfilePicture = row.Field<string>("image_path");
                    continue;
                }

                profilePictures.Add(row.Field<string>("image_path"));
            }

            //get amount of profile views
            var profileViewsAmount = profileViews.Rows.Count;
            if (userInfo.Rows.Count > 0)
            {
                var user = CreateUserEntityFromTable(userInfo);
                user.Profile = new ProfileEntity
                {
                    ProfileId = userInfo.Rows[0].Field<int>("profile_id"),
                    Gender = userInfo.Rows[0].Field<string>("gender"),
                    SexualPreferences = userInfo.Rows[0].Field<string>("sexual_preferences"),
                    Biography = userInfo.Rows[0].Field<string>("biography"),
                    ProfilePicture = mainProfilePicture,
                    ProfilePictureId = userInfo.Rows[0].Field<int>("profile_picture_id"),
                    FameRating = userInfo.Rows[0].Field<int>("fame_rating"),
                    Location = userInfo.Rows[0].Field<string>("location"),
                    Interests = userInterests,
                    Pictures = profilePictures,
                    ViewsAmount = profileViewsAmount,
                    LikesAmount = likesAmount,
                };
                return user;
            }

            return null;
        }

        private UserEntity? CreateUserEntityFromTable(DataTable userInfo)
        {
            return new UserEntity
            {
                UserId = userInfo.Rows[0].Field<int>("user_id"),
                UserName = userInfo.Rows[0].Field<string>("user_name"),
                FirstName = userInfo.Rows[0].Field<string>("first_name"),
                LastName = userInfo.Rows[0].Field<string>("last_name"),
                Email = userInfo.Rows[0].Field<string>("email"),
                Password = userInfo.Rows[0].Field<string>("password"),
                UpdatedAt = userInfo.Rows[0].Field<DateTime?>("updated_at"),
                CreatedAt = userInfo.Rows[0].Field<DateTime?>("created_at"),
                LastLoginAt = userInfo.Rows[0].Field<DateTime?>("last_login_at"),
                ResetTokenExpiry = userInfo.Rows[0].Field<DateTime?>("reset_token_expiry"),
                ResetToken = userInfo.Rows[0].Field<string>("reset_token"),
                IsVerified = userInfo.Rows[0].Field<bool>("is_verified")
            };
        }

        private static async Task<DataTable> GetTableByParameter(SqliteConnection connection, string sqlQuery,
            string parameterName,
            int parameter)
        {
            SqliteCommand command;
            command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            command.Parameters.AddWithValue(parameterName, parameter);
            var interestsReader = await command.ExecuteReaderAsync();
            var interstTable = new DataTable();
            interstTable.Load(interestsReader);
            return interstTable;
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


        private static UserEntity CreateUserEntity(SqliteDataReader reader)
        {
            return new UserEntity
            {
                UserId = reader.GetInt32(0),
                UserName = reader.GetString(1),
                FirstName = reader.GetString(2),
                LastName = reader.GetString(3),
                Email = reader.GetString(4),
                Password = reader.GetString(5),
                UpdatedAt = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                CreatedAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                LastLoginAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                ResetTokenExpiry = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                ResetToken = reader.IsDBNull(10) ? null : reader.GetString(10),
                IsVerified = reader.GetBoolean(11)
            };
        }

        #endregion
    }
}