using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BLL.Helpers;
using DAL.Entities;
using DAL.Helpers;
using DAL.Repositories;
using Profile = DAL.Entities.Profile;

namespace BLL.Sevices;

public class ProfileService(
    UserRepository userRepository,
    ProfileRepository profileRepository,
    PicturesRepository picturesRepository,
    InterestsRepository interestsRepository,
    LikesRepository likesRepository,
    UserInterestsRepository userInterestsRepository,
    IMapper mapper)
{
    public async Task<User> GetFullProfileByIdAsync(int id)
    {
        var user = await profileRepository.GetFullProfileAsync(id);
        if (user == null) throw new ValidationException("User not found");

        var builder = new ProfileBuilder();
        builder.AddMainData(user);
        builder.AddProfilePictures(await picturesRepository.GetPicturesByUserIdAsync(user.UserId));
        builder.AddMainProfilePicture(await picturesRepository.GetProfilePictureAsync(user.Profile?.ProfilePictureId));
        builder.AddUserInterests(await interestsRepository.GetInterestsByUserIdAsync(id));
        return builder.Build();
    }

    public async Task<List<User>> GetFullProfilesAsync(SearchParameters search, SortParameters sort,
        PaginationParameters pagination)
    {
        var currentUser = await profileRepository.GetProfileByIdAsync(search.UserId);
        if(!currentUser.IsActive) throw new ValidationException("update user profile first");
        var users = await profileRepository.GetFullProfilesAsync(search, sort, pagination);
        if (users == null) return new List<User>();
        var builder = new ProfileBuilder();
        var usersList = new List<User>();
        foreach (var user in users)
        {
            builder.AddMainData(user);
            builder.AddMainProfilePicture(
                await picturesRepository.GetProfilePictureAsync(user.Profile?.ProfilePictureId));
            builder.AddUserInterests(await interestsRepository.GetInterestsByUserIdAsync(user.UserId));
            usersList.Add(builder.Build());
        }

        return usersList;
    }

    public async Task UpdateProfileAsync(int id, Profile profile)
    {
        //check that interests exist
        var allInterests= (await interestsRepository.GetInterestsAsync()).Select(i => i.Name);
        foreach (var interest in profile.Interests)
        {
            if (!allInterests.Contains(interest.Name)) await interestsRepository.CreateInterestAsync(interest.Name);
        }
        var currentProfile = await profileRepository.GetProfileByIdAsync(id);
        if (currentProfile == null) throw new ObjectNotFoundException("Profile not found");
        profile.ProfileId = id;
        var res = await profileRepository.UpdateProfileAsync(profile);
        if (res == null) throw new ObjectNotFoundException("Profile not found");
        //add/update interests
        var userInterestsByNamesAsync = await interestsRepository.GetUserInterestsByNamesAsync(profile.Interests.Select(i => i.Name));
        await interestsRepository.UpdateUserInterestsAsync(id, userInterestsByNamesAsync);
    }

    public async  void UploadPhoto(int id, byte[] filePicture, bool isMain)
    {
        //TODO add validation
        
        var isProfilePicture = isMain ? 1 : 0;
        var pictureId = picturesRepository.UploadPhoto(id, filePicture, isProfilePicture);
        if (isMain)
        {
            var profile = await profileRepository.GetProfileByIdAsync(id);
            profile.ProfilePictureId = pictureId;
            await profileRepository.UpdateProfilePicture(pictureId);
        }
    }

    public void DeletePhoto(int userId, int photoId, bool isMain)
    {
        //TODO add validation
        
        picturesRepository.DeletePhoto(userId, photoId);
        if (isMain)
        {
            var profile = profileRepository.GetProfileByIdAsync(userId).Result;
            if (profile.ProfilePictureId == photoId)
            {
                profileRepository.UpdateProfilePicture(null);
            }
        }
    }

    public async Task<List<Interest>> GetInterestsAsync()
    {
        //check that intersets are used
        var allInterests = await interestsRepository.GetInterestsAsync();
        var usersInterests = await userInterestsRepository.GetAllUserInterestsAsync();
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
}