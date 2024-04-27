using System.Data;
using DAL.Entities;

namespace DAL.Helpers;

public class EntityCreator(DataParser dataParser)
{
    
    private readonly DataParser _dataParser = dataParser;
    
    public  Profile CreateUserProfile(DataRow userInfoRow, string? mainProfilePicture, List<string> userInterests, List<string> profilePictures, int profileViewsAmount, int likesAmount)
        {
            return new Profile
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

    public User? CreateUser(DataRow row)
        {
            return new User
            {
                UserId = row.Field<long>("user_id"),
                IsVerified = row.IsNull("is_verified") ? (bool?)null : _dataParser.ConvertLongToBool(row.Field<long>("is_verified")),
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
}