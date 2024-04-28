using System.Data;
using DAL.Entities;

namespace DAL.Helpers;

public class EntityCreator(DataParser dataParser)
{
    private readonly DataParser _dataParser = dataParser;

    public Profile CreateUserProfile(DataRow userInfoRow)
    {
        return new Profile
        {
            ProfileId = userInfoRow.Field<long>("profile_id"),
            Gender = userInfoRow.IsNull("gender") ? null : userInfoRow.Field<string>("gender"),
            SexualPreferences = userInfoRow.IsNull("sexual_preferences")
                ? null
                : userInfoRow.Field<string>("sexual_preferences"),
            Biography = userInfoRow.IsNull("biography") ? null : userInfoRow.Field<string>("biography"),
            ProfilePictureId = userInfoRow.IsNull("profile_picture_id")
                ? null
                : userInfoRow.Field<long>("profile_picture_id"),
            FameRating = userInfoRow.IsNull("fame_rating") ? null : userInfoRow.Field<long>("fame_rating"),
            Location = userInfoRow.IsNull("location") ? null : userInfoRow.Field<string>("location"),
            Age = userInfoRow.Field<long>("age"),
        };
    }

    public User? CreateUser(DataRow row)
    {
        return new User
        {
            UserId = row.Field<long>("user_id"),
            IsVerified = row.IsNull("is_verified")
                ? null
                : _dataParser.ConvertLongToBool(row.Field<long>("is_verified")),
            UserName = row.Field<string>("user_name"),
            FirstName = row.Field<string>("first_name"),
            LastName = row.Field<string>("last_name"),
            Email = row.Field<string>("email"),
            Password = row.Field<string>("password"),
            UpdatedAt = _dataParser.TryParseDateTime(row.Field<string>("updated_at")),
            CreatedAt = _dataParser.TryParseDateTime(row.Field<string>("created_at")),
            LastLoginAt = _dataParser.TryParseDateTime(row.Field<string>("last_login_at")),
            ResetTokenExpiry = _dataParser.TryParseDateTime(row.Field<string>("reset_token_expiry")),
            ResetToken = row.Field<string?>("reset_token"),
        };
    }

    public Picture CreatePicture(DataRow row)
    {
        return new Picture
        {
            PictureId = row.Field<long>("picture_id"),
            UserId = row.Field<long>("user_id"),
            PicturePath = row.Field<string>("picture_path"),
            IsProfilePicture = row.Field<long>("is_profile_picture") == 1,
            CreatedAt = _dataParser.TryParseDateTime(row.Field<string>("created_at")),
            UpdatedAt = _dataParser.TryParseDateTime(row.Field<string>("updated_at")),
        };
    }

    public Like CreateLikes(DataRow row)
    {
        return new Like
        {
            LikerId = row.Field<long>("liker_user_id"),
            LikedId = row.Field<long>("liked_user_id"),
            LikerUserName = row.Field<string>("liker_user_name"),
            LakedAt = _dataParser.TryParseDateTime(row.Field<string>("liked_at")),
        };
    }

    public Interests CreateInterests(DataRow row)
    {
        return new Interests
        {
            Interest = row.Field<string>("name"),
        };
    }
}