using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BLL.Helpers;
using DAL.Entities;
using DAL.Helpers;
using DAL.Repositories;
using Web_API.Helpers;
using Profile = DAL.Entities.Profile;

namespace BLL.Sevices;

public class ProfileService(
    UsersRepository usersRepository,
    ProfilesRepository profilesRepository,
    PicturesRepository picturesRepository,
    InterestsRepository interestsRepository,
    LikesRepository likesRepository,
    UsersInterestsRepository usersInterestsRepository,
    IMapper mapper)
{
    public async Task<User> GetFullProfileByIdAsync(int id)
    {
        var user = await profilesRepository.GetFullProfileAsync(id);
        if (user == null) throw new DataValidationException("Actor not found");

        var builder = new ProfileBuilder();
        builder.AddMainData(user);
        builder.AddProfilePictures(await picturesRepository.GetPicturesByUserIdAsync(user.Id));
        builder.AddMainProfilePicture(await picturesRepository.GetProfileAsync(user.Profile?.ProfilePictureId));
        builder.AddUserInterests(await interestsRepository.GetInterestsByUserIdAsync(id));
        return builder.Build();
    }

    public async Task<(long, List<User>)> GetFullProfilesAsync(SearchParameters search, SortParameters sort,
        PaginationParameters pagination, int id)
    {
        var currentUser = await profilesRepository.GetProfileAsync(id);
        if(!currentUser.IsActive) throw new ForbiddenActionException("update user profile first");
        var tagsIds = await interestsRepository.GetUserInterestsByNamesAsync(search.CommonTags);
        search.SexualPreferences ??= currentUser.SexualPreferences;
        var (counter, users) = await profilesRepository.GetFullProfilesAsync(search, sort, pagination, id, tagsIds);
        if (users == null) return (0, new List<User>());
        var builder = new ProfileBuilder();
        var usersList = new List<User>();
        foreach (var user in users)
        {
            builder.AddMainData(user);
            builder.AddMainProfilePicture(
                await picturesRepository.GetProfileAsync(user.Profile?.ProfilePictureId));
            builder.AddUserInterests(await interestsRepository.GetInterestsByUserIdAsync(user.Id));
            usersList.Add(builder.Build());
        }

        var amountOfPages = (long)Math.Ceiling((double)counter / pagination.PageSize);
        return (amountOfPages, usersList);
    }

    public async Task UpdateProfileAsync(int id, Profile profile)
    {
        var allInterests= (await interestsRepository.GetInterestsAsync()).Select(i => i.Name);
        foreach (var interest in profile.Interests)
        {
            if (!allInterests.Contains(interest.Name)) await interestsRepository.CreateInterestAsync(interest.Name);
        }
        var currentProfile = await profilesRepository.GetProfileAsync(id);
        if (currentProfile == null) throw new ObjectNotFoundException("Profile not found");
        profile.Id = id;
        var res = await profilesRepository.UpdateProfileAsync(profile);
        if (res == null) throw new ObjectNotFoundException("Profile not found");
        //add/update interests
        var userInterestsByNamesAsync = await interestsRepository
            .GetUserInterestsByNamesAsync(profile.Interests
                .Select(i => i.Name)
                .ToList());
        await interestsRepository.UpdateUserInterestsAsync(id, userInterestsByNamesAsync);
    }

    public async  Task UploadPhotoAsync(int userId, byte[] filePicture, bool isMain)
    {
      
        var isProfilePicture = isMain ? 1 : 0;
        if (isMain)
        {
            var user = await profilesRepository.GetProfileAsync(userId);
            if (user.ProfilePictureId != 501)
            {
                await picturesRepository.DeletePhotoAsync(userId, (int)user.ProfilePictureId);
            }
        }
        var pictureId = await picturesRepository.UploadPhoto(userId, filePicture, isProfilePicture);
        if (isMain)
        {
            await profilesRepository.UpdateProfilePictureAsync( userId, pictureId);
        }
    }

    public async Task DeletePhotoASync(int userId, int photoId, bool isMain)
    {
        //TODO add validation
        
        await picturesRepository.DeletePhotoAsync(userId, photoId);
        if (isMain)
        {
            var profile = profilesRepository.GetProfileAsync(userId).Result;
            if (profile.ProfilePictureId == photoId)
            {
                await profilesRepository.UpdateProfilePictureAsync(userId, photoId);
            }
        }
    }

    public async Task<List<Interest>> GetInterestsAsync()
    {
        var allInterests = await interestsRepository.GetInterestsAsync();
        var usersInterests = await usersInterestsRepository.GetAllUserInterestsAsync();
        var usedInterests = usersInterests.Select(i => i.InterestId);
        foreach (var interest in allInterests)
        {
            if (!usedInterests.Contains(interest.InterestId))
            {
                await interestsRepository.DeleteInterestByIdAsync(interest.InterestId);
            }
        }
        
        return await interestsRepository.GetInterestsAsync();
    }

    public async Task<User> CheckUserLike(User user, int viewerId)
    {
            user.HasLike = await likesRepository.HasLike(viewerId, user.Id);
            return user;
    }
    
    public async Task<List<User>> CheckUsersLikes(List<User> users, int viewerId)
    {
        foreach (var user in users)
        {
            user.HasLike = await likesRepository.HasLike(viewerId, user.Id);
        }

        return users;
    }

    public async Task<FiltersData> GetFiltersAsync(int id)
    {
        var user = await profilesRepository.GetProfileAsync(id);
        if (!user.IsActive) throw new ForbiddenActionException("update user profile first");
        return await profilesRepository.GetFiltersDataAsync(user.Longitude, user.Latitude, id);

    }

    public async Task<bool> IsMatch(int producerId, int consumerId)
    {
        var producerLikes = await likesRepository.GetLikesByUserIdAsync(producerId);
        var consumerLikes = await likesRepository.GetLikesByUserIdAsync(consumerId);
        return producerLikes.Any(l => l.LikerId == consumerId) &&
               consumerLikes.Any(l => l.LikerId == producerId);
    }
}