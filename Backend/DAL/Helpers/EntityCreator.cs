using System.Data;
using DAL.Entities;

namespace DAL.Helpers;

public class EntityCreator
{
    public Profile CreateUserProfile(DataRow userInfoRow)
    {
        return new Profile
        {
            Id = userInfoRow.Field<int>("profile_id"),
            Gender = userInfoRow.IsNull("gender") ? null : userInfoRow.Field<string>("gender"),
            SexualPreferences = userInfoRow.IsNull("sexual_preferences")
                ? null
                : userInfoRow.Field<string>("sexual_preferences"),
            Biography = userInfoRow.IsNull("biography") ? null : userInfoRow.Field<string>("biography"),
            ProfilePictureId = userInfoRow.IsNull("profile_picture_id")
                ? null
                : userInfoRow.Field<int>("profile_picture_id"),
            FameRating = userInfoRow.IsNull("fame_rating") ? null : userInfoRow.Field<int>("fame_rating"),
            Latitude = userInfoRow.IsNull("latitude") ? null : userInfoRow.Field<float>("latitude"),
            Longitude = userInfoRow.IsNull("longitude") ? null : userInfoRow.Field<float>("longitude"),
            Age = userInfoRow.Field<int>("age"),
            IsActive = userInfoRow.Field<bool>("is_active"),
        };
    }

    public User? CreateUser(DataRow row)
    {
        return new User
        {
            Id = row.Field<int>("user_id"),
            IsVerified = row.Field<bool>("is_verified"),
            UserName = row.Field<string>("user_name"),
            FirstName = row.Field<string>("first_name"),
            LastName = row.Field<string>("last_name"),
            Email = row.Field<string>("email"),
            Password = row.Field<string>("password"),
            EmailResetToken = row.Field<string?>("email_reset_token"),
            JwtResetToken = row.Field<string?>("jwt_reset_token"),
        };
    }

    public Picture CreatePicture(DataRow row)
    {
        return new Picture
        {
            Id = row.Field<int>("picture_id"),
            UserId = row.Field<int>("user_id"),
            PicturePath = row.Field<byte[]>("picture_path"),
            IsProfilePicture = row.Field<int>("is_profile_picture") == 1,
        };
    }

    public Like CreateLikes(DataRow row)
    {
        return new Like
        {
            LikerId = row.Field<int>("liker_user_id"),
            LikedId = row.Field<int>("liked_user_id"),
        };
    }

    public Interest CreateInterests(DataRow row)
    {
        return new Interest
        {
            InterestId = row.Field<int>("interest_id"),
            Name = row.Field<string>("name"),
        };
    }

    public IEnumerable<User>? CreateUsers(DataTable dataTable)
    {
        var users = new List<User>();
        foreach (DataRow row in dataTable.Rows)
        {
            var user = CreateUser(row);
            AddDistance(row, user);
            user.Profile = CreateUserProfile(row);
            users.Add(user);
            
        }

        return users;
    }

    private void AddDistance(DataRow row, User? user)
    {
        if (user is null) return;
        if (row.IsNull("distance"))
        {
            user.Distance = 0;
        }
        else
        {
            user.Distance = row.Field<double>("distance");
        }  
    }

    public ProfileView CreateProfileViews(DataRow row)
    {
        return new ProfileView
        {
            ViewedId = row.Field<int>("profile_user_id"),
            ViewerId = row.Field<int>("viewer_user_id"),
        };
    }

    public UserInterest CreateUserInterest(DataRow row)
    {
        return new UserInterest
        {
            USerId = row.Field<int>("user_id"),
            InterestId = row.Field<int>("interest_id"),
        };
    }
    
    public FiltersData CreateFiltersData(DataRow row)
    {
        return new FiltersData
        {
            MaxAge = row.Field<int>("max_age"),
            MinAge = row.Field<int>("min_age"),
            MaxDistance = row.Field<double>("max_distance"),
            MinDistance = row.Field<double>("min_distance"),
            MaxFameRating = row.Field<int>("max_fame_rating"),
            MinFameRating = row.Field<int>("min_fame_rating")
        };
    }

    public Message CreateMessage(DataRow row)
    {
        return new Message
        {
            RoomId = row.Field<int>("room_id"),
            SenderId = row.Field<int>("sender_id"),
            Text = row.Field<string>("text"),
            Created_at = row.Field<DateTime>("time"),
        };
    }
}