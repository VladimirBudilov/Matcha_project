using BLL.Helpers;
using DAL.Entities;
using DAL.Helpers;
using DAL.Repositories;

namespace BLL.Sevices;

public class ActionService(
    LikesRepository likesRepository,
    ProfileRepository userRepository,
    ProfileViewsRepository profileViewRepository,
    FameRatingCalculator fameRatingCalculator
    )
{
    public async Task<Like> LikeUser(int likerId, int likedId)
    {
        //Check that user is not liking himself
        if (likerId == likedId) throw new ObjectNotFoundException("You can't like yourself");
        
        var likes = await likesRepository.GetLikesByUserIdAsync(likerId);
        Like output = null;
        foreach (var like in likes)
        {
            if (like.LikedId == likedId)
            {
                output = await likesRepository.DeleteLikesAsync(likerId, likedId);
                output.State = "unlike";
            }
        }

        if (output?.State != "unlike")
        {
            output = await likesRepository.CreateLikesAsync(new Like() { LikedId = likedId, LikerId = likerId });
        }

        //update fem rating
        await UpdateFameRating(likedId);

        return output;
    }

    public async Task ViewedUser(int viewerId, int viewedId)
    {
        //Check that user is not viewing himself
        if (viewerId == viewedId) return;
        
        //check that user is not viewing the same user twice
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
    }
}
