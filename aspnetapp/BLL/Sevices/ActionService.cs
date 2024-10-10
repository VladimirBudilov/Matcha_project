using BLL.Helpers;
using DAL.Entities;
using DAL.Helpers;
using DAL.Repositories;
using Web_API.Helpers;
using Web_API.Hubs.Helpers;

namespace BLL.Sevices;

public class ActionService(
    LikesRepository likesRepository,
    ProfilesRepository userRepository,
    ProfileViewsRepository profileViewRepository,
    BlackListRepository blackListRepository,
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

    public async Task<bool> TryViewUser(int viewerId, int viewedId)
    {
        if (viewerId == viewedId) return false;
        await validator.CheckUserExistence([viewerId, viewedId]);
        if (await profileViewRepository.GetView(viewerId, viewedId) != null) return true;

        await profileViewRepository.CreateProfileViewsAsync(new ProfileView()
            { ViewedId = viewedId, ViewerId = viewerId });

        await UpdateFameRating(viewedId);
        return true;
    }

    private async Task UpdateFameRating(int id)
    {
        var user = await userRepository.GetProfileAsync(id);
        var userViews = await profileViewRepository.GetProfileViewsByUserIdAsync(id);
        var userLikes = await likesRepository.GetLikesByUserIdAsync(id);
        user.FameRating = FameRatingCalculator.Calculate(userLikes.Count(), userViews.Count());
        await userRepository.UpdateFameRatingAsync(user);
    }

    public async Task<bool> TryUpdateBlackListAsync(int actorId, int consumerId)
    {
        if (await CheckIfUserIsBlocked(consumerId, actorId))
        {
            await blackListRepository.DeleteFromBlackListAsync(actorId, consumerId);
            return false;
        }
        
        await blackListRepository.AddToBlackListAsync(new BlackList()
            { UserId = actorId, BlacklistedUserId = consumerId });
        return true;
    }

    public async Task<bool> CheckIfUserIsBlocked(int blockedId, int userId)
    {
        await validator.CheckUserExistence(new[] { blockedId, userId });
        
        var blackList = await blackListRepository.GetFromBlackListByIdAsync(userId);
        return blackList.Any(x => x.BlacklistedUserId == blockedId);
    }

    public async Task<List<int>> GetBlockedUsersIdASync(int userId)
    {
        var blackList = await blackListRepository.GetFromBlackListByIdAsync(userId);
        return blackList.Select(x => x.BlacklistedUserId).ToList();
    }
    
    public async Task<List<int>> GetForbiddenUsersIdASync(int userId)
    {
        var blackList = await blackListRepository.GetFromBlackListByBlockedIdAsync(userId);
        return blackList.Select(x => x.UserId).ToList();
    }   

}