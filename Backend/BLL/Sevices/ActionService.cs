using BLL.Helpers;
using DAL.Entities;
using DAL.Helpers;
using DAL.Repositories;
using Web_API.Helpers;

namespace BLL.Sevices;

public class ActionService(
    LikesRepository likesRepository,
    ProfileRepository userRepository,
    ProfileViewsRepository profileViewRepository,
    FameRatingCalculator fameRatingCalculator,
    ServiceValidator validator
    )
{
    public async Task<Like> LikeUser(int likerId, int likedId)
    {
        await validator.CheckUserExistence(new[]{likerId, likedId});

        if (likerId == likedId) throw new DataValidationException("You can't like yourself");
        var like = await likesRepository.GetLikeAsync(likerId, likedId);
        
        if (like.IsLiked)
        {
            like = await likesRepository.CreateLikesAsync(new Like() { LikedId = likedId, LikerId = likerId });
        }
        else
        {
            await likesRepository.DeleteLikeAsync(likerId, likedId);
        }
    
        await UpdateFameRating(likedId);
        return like;
    }

    public async Task ViewUser(int viewerId, int viewedId)
    {
        if (viewerId == viewedId) return;
        
        await validator.CheckUserExistence(new[]{viewerId, viewedId});

        
        var views = await profileViewRepository.GetProfileViewsByUserIdAsync(viewerId);
        foreach (var view in views)
        {
            if (view.ViewedId == viewedId) return;
        }
        
        await profileViewRepository.CreateProfileViewsAsync(new ProfileView() { ViewedId = viewedId, ViewerId = viewerId });

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
