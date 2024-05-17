using System.Data;
using DAL.Entities;
using DAL.Helpers;
using Npgsql;

namespace DAL.Repositories;

public class ProfileRepository(
    DatabaseSettings configuration,
    InterestsRepository interestsRepository,
    EntityCreator entityCreator,
    TableFetcher fetcher,
    QueryBuilder queryBuilder)
{
    private readonly string _connectionString = configuration.ConnectionString
                                                ?? throw new ArgumentNullException(nameof(configuration),
                                                    "Connection string not found in configuration");

    public async Task<User?> GetFullProfileAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var userInfo = await fetcher.GetTableByParameter(connection, "SELECT * FROM users" +
                                                                     " LEFT JOIN profiles ON users.user_id = profiles.profile_id" +
                                                                     " WHERE user_id = @id", "@id", id);
        if (userInfo.Rows.Count > 0)
        {
            var userInfoRow = userInfo.Rows[0];
            var user = entityCreator.CreateUser(userInfoRow);
            user.Profile = entityCreator.CreateUserProfile(userInfoRow);
            return user;
        }

        return null;
    }

    public async Task<Profile> GetProfileByIdAsync(long id)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await fetcher.GetTableByParameter(connection,
                "SELECT * FROM profiles WHERE profile_id = @id", "@id", id);
            return dataTable.Rows.Count > 0 ? entityCreator.CreateUserProfile(dataTable.Rows[0]) : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while getting profile by id", e);
        }
    }

    public async Task<Profile?> CreateProfileAsync(Profile entity)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO profiles (profile_id, gender, age) " +
                " VALUES (@profile_id, @gender, @age)";
            command.Parameters.AddWithValue("@profile_id", entity.ProfileId);
            command.Parameters.AddWithValue("@gender", entity.Gender);
            command.Parameters.AddWithValue("@age", entity.Age);
            var res = await command.ExecuteNonQueryAsync();
            return res > 0 ? entity : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while adding profile", e);
        }
    }

    public async Task<int> UpdateProfilePicture(int pictureId)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText =
                "UPDATE profiles SET " +
                "profile_picture_id = @profile_picture_id " +
                " WHERE profile_id = @profile_id";
            
            command.Parameters.AddWithValue("@profile_picture_id", pictureId);
            var res = await command.ExecuteNonQueryAsync();
            return res;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while updating profile photo", e);
        }
    }
    public async Task<Profile> UpdateProfileAsync(Profile entity)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText =
                "UPDATE profiles SET gender = @gender," +
                " sexual_preferences = @sexual_preferences, " +
                "biography = @biography, age = @age, " +
                "profile_picture_id = @profile_picture_id " +
                "latitude = @latitude, longitude = @longitude" +
                " WHERE profile_id = @profile_id";

            command.Parameters.AddWithValue("@profile_id", entity.ProfileId);
            command.Parameters.AddWithValue("@gender", entity.Gender);
            command.Parameters.AddWithValue("@sexual_preferences", entity.SexualPreferences);
            command.Parameters.AddWithValue("@biography", entity.Biography);
            command.Parameters.AddWithValue("@age", entity.Age);
            command.Parameters.AddWithValue("@latitude", entity.Latitude);
            command.Parameters.AddWithValue("@longitude", entity.Longitude);
            command.Parameters.AddWithValue("@profile_picture_id", entity.ProfilePictureId);

            var res = await command.ExecuteNonQueryAsync();
            //get full data about user and check that it does nor contain nulls
            if (!(await GetProfileByIdAsync(entity.ProfileId)).HasEmptyFields())
            {
                command.CommandText =
                    "UPDATE profiles SET is_active = TRUE " +
                    " WHERE profile_id = @profile_id";
                await command.ExecuteNonQueryAsync();
            }
            // Deconstruct profile
            return res > 0 ? entity : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while updating profile", e);
        }
    }

    public async Task<IEnumerable<User>?> GetFullProfilesAsync(SearchParameters searchParams, SortParameters sortParams,
        PaginationParameters pagination)
    {
        var profile = (await GetFullProfileAsync(searchParams.UserId))!.Profile;
        /*try
        {*/
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        var userInterests = await interestsRepository.GetInterestsByUserIdAsync(searchParams.UserId);
        var userInterestsIds = userInterests.Select(interest => interest.InterestId).ToArray();
        queryBuilder.Select($" users.user_id as user_id, users.*, profiles.*, count_common_elements(@userInterests, ARRAY_AGG(interests.interest_id)) AS common_interests ");
        queryBuilder.From(" users \n JOIN profiles ON users.user_id = profiles.profile_id ");
        queryBuilder.From(" LEFT JOIN user_interests ON user_interests.user_id = users.user_id ");
        queryBuilder.From(" LEFT JOIN interests ON user_interests.interest_id = interests.interest_id ");
        //add filters
        ApplyFilters(searchParams, profile, command);
        //add group by
        queryBuilder.GroupBy(" users.user_id, profiles.profile_id ");
        //add sorting
        var parameters = new List<string> { 
            $"calculate_distance(latitude,longitude,{profile.Latitude},{profile.Longitude})",
            "fame_rating",
            "age",
            $"common_interests" 
        };
        var sortType = sortParams.ToList();
        queryBuilder.OrderBy($" {parameters[sortParams.SortingMainParameter]} {sortType[sortParams.SortingMainParameter]}, ");
        parameters.RemoveAt(sortParams.SortingMainParameter);
        sortType.RemoveAt(sortParams.SortingMainParameter);
        for (var i = 0; i < parameters.Count; i++)
        {
            queryBuilder.OrderBy($" {parameters[i]} {sortType[i]} ");
            if (i != parameters.Count - 1)
            {
                queryBuilder.OrderBy(", ");
            }
        }
        //add pagination
        queryBuilder.Limit($" {pagination.PageSize} ");
        queryBuilder.Offset($" {(pagination.PageNumber -1) * pagination.PageSize} ");

        command.CommandText = queryBuilder.Build();
        command.Parameters.AddWithValue("@userInterests", userInterestsIds);
        var dataTable = new DataTable();
        var reader = await command.ExecuteReaderAsync();
        dataTable.Load(reader);
        return dataTable.Rows.Count > 0 ? entityCreator.CreateUsers(dataTable) : null;
        /*}
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while getting full profiles", e);
        }*/
    }

    private void ApplyFilters(SearchParameters searchParams, Profile profile, NpgsqlCommand command)
    {
        queryBuilder.Where($" users.user_id != {profile.ProfileId} AND profiles.is_active = TRUE AND users.is_verified = TRUE ");
        if(searchParams.MaxDistance!= null)
        {
            queryBuilder.Where($" AND calculate_distance(latitude,longitude,{profile.Latitude},{profile.Longitude} < {searchParams.MaxDistance} ");
        }

        if (searchParams.SexualPreferences != null)
        {
            queryBuilder.Where($" AND sexual_preferences != {searchParams.SexualPreferences} ");
        }

        if (searchParams.CommonTags.Count != 0)
        {
            queryBuilder.Where($" AND count_shared_elements(@filterTags, ARRAY_AGG(interests.name)) >= {searchParams.CommonTags.Count}");
            command.Parameters.AddWithValue("@filterTags", searchParams.CommonTags);
        }
        
        if (searchParams.MinAge != null && searchParams.MaxAge != null)
        {
            queryBuilder.Where($" AND age BETWEEN {searchParams.MinAge} AND {searchParams.MaxAge} ");
        }
        
        if (searchParams.MinFameRating != null && searchParams.MaxFameRating != null)
        {
            queryBuilder.Where($" AND fame_rating BETWEEN {searchParams.MinFameRating} AND {searchParams.MaxFameRating} ");
        }

        if (searchParams.IsLikedUser != null)
        {
            queryBuilder.Where(searchParams.IsLikedUser == true
                ? $" AND has_user_liked(users.user_id, {profile.ProfileId})"
                : $" AND NOT has_user_liked(users.user_id, {profile.ProfileId})");
        }

        if (searchParams.IsMatched == null) return;
        queryBuilder.Where(searchParams.IsMatched == true
            ? $" AND users_matched(users.user_id, {profile.ProfileId})"
            : $" AND NOT users_matched(users.user_id, {profile.ProfileId})");
    }
}