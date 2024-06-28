using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;

namespace DAL.Repositories;

public class ProfilesRepository(
    InterestsRepository interestsRepository,
    EntityCreator entityCreator,
    TableFetcher fetcher)
{
    public async Task<User?> GetFullProfileAsync(int id)
    {
        var query = new StringBuilder().Append("SELECT * FROM users")
            .Append(" LEFT JOIN profiles ON users.user_id = profiles.profile_id")
            .Append(" WHERE user_id = @id");
        var userInfo = await fetcher.GetTableByParameter(query.ToString(), "@id", id);
        if (userInfo.Rows.Count > 0)
        {
            var userInfoRow = userInfo.Rows[0];
            var user = entityCreator.CreateUser(userInfoRow);
            user.Profile = entityCreator.CreateUserProfile(userInfoRow);
            return user;
        }

        return null;
    }

    public async Task<Profile> GetProfileAsync(int id)
    {
        try
        {
            var dataTable =
                await fetcher.GetTableByParameter("SELECT * FROM profiles WHERE profile_id = @id", "@id", id);
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
            var query =new StringBuilder()
                .Append("INSERT INTO profiles (profile_id, gender, age) ")
                .Append(" VALUES (@profile_id, @gender, @age) ")
                .Append("Returning profile_id");
            var parametrs = new Dictionary<string, object>()
            {
                { "profile_id", entity.Id },
                { "gender", entity.Gender },
                { "age", entity.Age }
            };
            var table = await fetcher.GetTableByParameter(query.ToString(), parametrs);
            return table.Rows.Count > 0 ? entity : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while adding profile", e);
        }
    }

    public async Task<int> UpdateProfilePictureAsync(int userId, int pictureId)
    {
        try
        {
            var query =
                "UPDATE profiles SET profile_picture_id = @profile_picture_id WHERE profile_id = @profile_id RETURNING profile_picture_id";
            var table = await fetcher.GetTableByParameter(query, new Dictionary<string, object>
            {
                { "@profile_picture_id", pictureId },
                { "@profile_id", userId }
            });
            return table.Rows.Count > 0 ? pictureId : -1;
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
            var query = new StringBuilder()
                .Append("UPDATE profiles SET gender = @gender,")
                .Append(" sexual_preferences = @sexual_preferences, ")
                .Append("biography = @biography, age = @age, ")
                .Append("profile_picture_id = @profile_picture_id, ")
                .Append("latitude = @latitude, longitude = @longitude ")
                .Append("WHERE profile_id = @profile_id ")
                .Append("RETURNING is_active");
            var parameters = new Dictionary<string, object>()
            {
                { "@profile_id", entity.Id },
                { "@gender", entity.Gender },
                { "@sexual_preferences", entity.SexualPreferences },
                { "@biography", entity.Biography },
                { "@age", entity.Age },
                { "@latitude", entity.Latitude },
                { "@longitude", entity.Longitude }
            };

            var table = await fetcher.GetTableByParameter(query.ToString(), parameters);
            /*var isActive = (bool)table.Rows[0]["is_active"];
            if (!isActive && !(await GetProfileAsync(entity.Id)).HasEmptyFields())
            {*/
                query = new StringBuilder()
                    .Append("UPDATE profiles SET is_active = TRUE ")
                    .Append(" WHERE profile_id = @profile_id");
                await fetcher.GetTableAsync(query.ToString());
            //}

            return entity;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while updating profile", e);
        }
    }

    public async Task<(long, IEnumerable<User>?)> GetFullProfilesAsync(SearchParameters searchParams,
        SortParameters sortParams,
        PaginationParameters pagination, int id, List<Interest> tagsIds)
    {
        var profile = (await GetFullProfileAsync(id))!.Profile;
        /*try
        {*/
        var userInterests = await interestsRepository.GetInterestsByUserIdAsync(id);
        var userInterestsIds = userInterests.Select(interest => interest.InterestId).ToArray();
        var queryBuilder = new QueryBuilder();
        queryBuilder
            .Select(" users.user_id as user_id, users.*, profiles.*, ")
            .Select("calculate_distance(@profile_latitude, @profile_longitude, profiles.latitude, profiles.longitude) as distance, ")
            .Select("count_common_elements(@userInterests, ARRAY_AGG(interests.interest_id)) AS common_interests ")
            .From("users JOIN profiles ON users.user_id = profiles.profile_id ")
            .From("LEFT JOIN user_interests ON user_interests.user_id = users.user_id ")
            .From("LEFT JOIN interests ON user_interests.interest_id = interests.interest_id ");
        var parameters = new Dictionary<string, object>
        {
            { "@profile_latitude", (double)profile.Latitude },
            { "@profile_longitude", (double)profile.Longitude },
            { "@userInterests", userInterestsIds },
            {"@profile_id", id}

        };

        //add filters
        ApplyFilters(searchParams, tagsIds, parameters, ref queryBuilder);
        //add group by
        ApplyOrdering(sortParams, ref queryBuilder);
        //add pagination
        var numberOfUsers = await GetNumberOfUsers(queryBuilder, parameters);
        if (numberOfUsers == 0) return (0, new List<User>());

        queryBuilder.Limit(" @pageSize ");
        queryBuilder.Offset(" @offset ");
        parameters.Add("@pageSize", pagination.PageSize);
        parameters.Add("@offset", (pagination.PageNumber - 1) * pagination.PageSize);

        var query = queryBuilder.Build();
        
        var table = await fetcher.GetTableByParameter(query, parameters);
        var users = new List<User>();
        foreach (DataRow row in table.Rows)
        {
            var user = entityCreator.CreateUser(row);
            user.Profile = entityCreator.CreateUserProfile(row);
            users.Add(user);
        }

        return (numberOfUsers, users);
        /*}
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while getting full profiles", e);
        }*/
    }

    private async Task<long> GetNumberOfUsers(QueryBuilder queryBuilder,
        Dictionary<string, object> parameters)
    {
        var counter = new QueryBuilder()
            .Select("Count(*) ")
            .From($" ({queryBuilder.Build()})");
        var query = counter.Build();
        var countCommand = await fetcher.GetTableByParameter(query , parameters);
        var count = (long)countCommand.Rows[0][0];
        return count;
    }

    private static void ApplyOrdering(SortParameters sortParams, ref QueryBuilder queryBuilder)
    {
        queryBuilder.GroupBy(" users.user_id, profiles.profile_id ");
        var parameters = new List<string>
        {
            $"distance",
            "fame_rating",
            "age",
            $"common_interests"
        };
        var sortType = sortParams.ToList();
        queryBuilder.OrderBy(
            $" {parameters[sortParams.SortingMainParameter]} {sortType[sortParams.SortingMainParameter]}, ");
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
    }

    private static void ApplyFilters(SearchParameters searchParams,
        List<Interest> tagsIds,
        Dictionary<string, object> parameters,
        ref QueryBuilder queryBuilder)
    {
        queryBuilder.Where($"users.user_id != @profile_id AND profiles.is_active = TRUE AND users.is_verified = TRUE ");
        if (searchParams.MaxDistance != null)
        {
            queryBuilder.Where(
                $"AND calculate_distance(latitude,longitude,@profile_latitude, @profile_longitude) <= @maxDistance ");
            parameters.Add("@maxDistance", searchParams.MaxDistance);
        }

        if (searchParams.SexualPreferences != null)
        {
            queryBuilder.Where($" AND sexual_preferences = @sexualPreferences ");
            parameters.Add("@sexualPreferences", searchParams.SexualPreferences);
        }

        if (searchParams.CommonTags.Count != 0)
        {
            queryBuilder.Where(" AND interests.interest_id = ANY(@commonTags) ");
            parameters.Add("@commonTags", tagsIds.Select(x => x.InterestId).ToArray());
        }

        if (searchParams.MinAge != null && searchParams.MaxAge != null)
        {
            queryBuilder.Where($" AND age BETWEEN @minAge AND @maxAge ");
            parameters.Add("@minAge", searchParams.MinAge);
            parameters.Add("@maxAge", searchParams.MaxAge);
        }

        if (searchParams.MinFameRating != null && searchParams.MaxFameRating != null)
        {
            queryBuilder.Where($" AND fame_rating BETWEEN @minFameRating AND @maxFameRating ");
            parameters.Add("@minFameRating", searchParams.MinFameRating);
            parameters.Add("@maxFameRating", searchParams.MaxFameRating);
        }

        if (searchParams.IsLikedUser != null)
        {
            queryBuilder.Where(searchParams.IsLikedUser == true
                ? $" AND has_user_liked(users.user_id, @profile_id)"
                : $" AND NOT has_user_liked(users.user_id, @profile_id)");
        }

        if (searchParams.IsMatched == null) return;
        queryBuilder.Where(searchParams.IsMatched == true
            ? $" AND users_matched(users.user_id, @profile_id)"
            : $" AND NOT users_matched(users.user_id, @profile_id)");
    }

    public async Task UpdateFameRatingAsync(Profile user)
    {
        try
        {
            var query = "UPDATE profiles SET fame_rating = @fame_rating WHERE profile_id = @profile_id";
            var parameters = new Dictionary<string, object>
            {
                { "@fame_rating", user.FameRating },
                { "@profile_id", user.Id }
            };
            await fetcher.GetTableByParameter(query, parameters);
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while updating fame rating", e);
        }
    }

    public async Task<FiltersData> GetFiltersDataAsync(double? longitude, double? latitude, int id)
    {
        var query = new StringBuilder()
            .Append("select max(age) as max_age, min(age) as min_age, ")
            .Append("min(fame_rating) as min_fame_rating, max(fame_rating) as max_fame_rating, ")
            .Append(
                "min(calculate_distance(@profile_latitude, @profile_longitude, profiles.latitude, profiles.longitude)) as min_distance, ")
            .Append(
                "max(calculate_distance(@profile_latitude, @profile_longitude, profiles.latitude, profiles.longitude)) as max_distance ")
            .Append("from profiles ")
            .Append(" where profiles.is_active = TRUE AND profiles.profile_id != @profile_id ");
        var parameters = new Dictionary<string, object>
        {
            { "@profile_latitude", (double)latitude! },
            { "@profile_longitude", (double)longitude! },
            { "@profile_id", id }
        };
        var table = await fetcher.GetTableByParameter(query.ToString(), parameters);
        return entityCreator.CreateFiltersData(table.Rows[0]);
    }

    
}