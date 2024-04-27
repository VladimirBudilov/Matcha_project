using System.Data;
using DAL.Entities;
using DAL.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;

namespace DAL.Repositories
{
    public class UserRepository(IConfiguration configuration, EntityCreator entityCreator, TableFetcher fetcher, ParameterInjector injector)
    {
        private readonly string _connectionString = configuration.GetConnectionString("UserDbConnection")
                                                    ?? throw new ArgumentNullException(nameof(configuration),
                                                        "Connection string not found in configuration");
        private readonly EntityCreator _entityCreator = entityCreator;
        private readonly TableFetcher _fetcher = fetcher;
        private readonly ParameterInjector _injector = injector;
        
        #region GettingData

        public async Task<UserEntity?> GetUserByIdAsync(int id)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await _fetcher.GetTableByParameter(connection, "SELECT * FROM users WHERE user_id = @id", "@id", id);
            return dataTable.Rows.Count > 0 ? _entityCreator.CreateUser(dataTable.Rows[0]) : null;
        }

        public async Task<UserEntity> GetUserByEmailAsync(string email)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var dataTable = await _fetcher.GetTableByParameter(connection, "SELECT * FROM users WHERE email = @email", "@email", email);
                return dataTable.Rows.Count > 0 ? _entityCreator.CreateUser(dataTable.Rows[0]) : null;
            }
        }

        public async Task<UserEntity?> GetUserByUserNameAsync(string userName)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await _fetcher.GetTableByParameter(connection, "SELECT * FROM users WHERE user_name = @userName", "@userName", userName);
            return dataTable.Rows.Count > 0 ? _entityCreator.CreateUser(dataTable.Rows[0]) : null;
        }

        public async Task<IEnumerable<UserEntity>> GetAllUsers()
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await _fetcher.GetTableByParameter(connection, "SELECT * FROM users", null, null);
            var users = new List<UserEntity>();
            foreach (DataRow row in dataTable.Rows)
            {
                users.Add(_entityCreator.CreateUser(row));
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
            var userInfo = await fetcher.GetTableByParameter(connection, "SELECT * FROM users" +
                                                                        " LEFT JOIN profiles ON users.user_id = profiles.profile_id" +
                                                                        " WHERE user_id = @id", "@id", id);
            var userInterestsById = await fetcher.GetTableByParameter(connection, "SELECT * FROM user_interests WHERE user_id = @id",
                "@id", id);
            var interestsIds = (from DataRow row in userInterestsById.Rows select row.Field<long>("interest_id")).ToList();
            command.CommandText = "SELECT * FROM interests WHERE interest_id IN (" +
                                  string.Join(",", interestsIds) + ")";
            var interests = new DataTable();
            interests.Load(await command.ExecuteReaderAsync());
            var likes = await fetcher.GetTableByParameter(connection, "SELECT * FROM likes WHERE liked_user_id = @id", "@id",
                id);
            var pictures =
                await fetcher.GetTableByParameter(connection, "SELECT * FROM pictures WHERE user_id = @id", "@id", id);
            var profileViews = await fetcher.GetTableByParameter(connection,
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
                var user = _entityCreator.CreateUser(userInfoRow);
                user.Profile = _entityCreator.CreateUserProfile(userInfoRow, mainProfilePicture, userInterests, profilePictures, profileViewsAmount, likesAmount);
                return user;
            }
            return null;
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
            _injector.FillUserEntityParameters(user, command);
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
            _injector.FillUserEntityParameters(user, command);
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
    }
}