using DAL.Entities;

namespace BLL.Helpers;

public class ProfileBuilder
{
    private User _user = new User();
    public void AddMainData(User getFullProfileAsync)
    {
        _user = getFullProfileAsync;
    }

    public User Build()
    {
        return _user;
    }

    public void AddMainProfilePicture(Picture picture)
    {
        _user.Profile.ProfilePicture = picture;
    }
    public void AddProfilePictures(IEnumerable<Picture> getPictureByUserIdAsync)
    {
        _user.Profile.Pictures.AddRange(getPictureByUserIdAsync);
    }

    public void AddUserInterests(IEnumerable<Interest> getUserInterestsByUserIdAsync)
    {
        _user.Profile.Interests.AddRange(getUserInterestsByUserIdAsync);
    }

    public void AddLikes(IEnumerable<Like> getLikesByUserIdAsync)
    {
        _user.Profile.Likes.AddRange(getLikesByUserIdAsync);
    }
}