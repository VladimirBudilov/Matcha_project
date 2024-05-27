using BLL.Helpers;
using DAL.Entities;
using DAL.Helpers;
using DAL.Repositories;
using Web_API.Helpers;
using Web_API.Hubs.Helpers;

namespace BLL.Sevices;

public class ActionService(
    LikesRepository likesRepository,
    ProfileRepository userRepository,
    ProfileViewsRepository profileViewRepository,
    FameRatingCalculator fameRatingCalculator,
    ServiceValidator validator
)
{
    public async Task<(string, Like)> LikeUser(int likerId, int likedId)
    {
        await validator.CheckUserExistence(new[] { likerId, likedId });

        if (likerId == likedId) throw new DataValidationException("You can't like yourself");
        var like = await likesRepository.GetLikeAsync(likerId, likedId);
        var notificationType = NotificationType.Like;
        if (like.IsLiked)
        {
            like = await likesRepository.CreateLikesAsync(new Like() { LikedId = likedId, LikerId = likerId });
            var likeByLiked = await likesRepository.GetLikeAsync(likedId, likerId);
            if (!likeByLiked.IsLiked)
            {
                notificationType = NotificationType.ResponseLike;
            }
        }
        else
        {
            await likesRepository.DeleteLikeAsync(likerId, likedId);
            notificationType = NotificationType.Unlike;
        }

        await UpdateFameRating(likedId);
        return (notificationType, like);
    }

    public async Task ViewUser(int viewerId, int viewedId)
    {
        if (viewerId == viewedId) return;

        await validator.CheckUserExistence([viewerId, viewedId]);


        var views = await profileViewRepository.GetProfileViewsByUserIdAsync(viewerId);
        foreach (var view in views)
        {
            if (view.ViewedId == viewedId) return;
        }

        await profileViewRepository.CreateProfileViewsAsync(new ProfileView()
            { ViewedId = viewedId, ViewerId = viewerId });

        await UpdateFameRating(viewedId);
    }

    private async Task UpdateFameRating(int id)
    {
        var user = await userRepository.GetProfileByIdAsync(id);
        var userViews = await profileViewRepository.GetProfileViewsByUserIdAsync(id);
        var userLikes = await likesRepository.GetLikesByUserIdAsync(id);
        user.FameRating = fameRatingCalculator.Calculate(userLikes.Count(), userViews.Count());
        await userRepository.UpdateFameRatingAsync(user);
    }
}