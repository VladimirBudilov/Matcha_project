﻿using AutoMapper;
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
    IMapper mapper)
{
    public async Task<User> GetFullProfileByIdAsync(long id)
    {
        var user = await profileRepository.GetFullProfileAsync(id);
        if (user == null) throw new ObjectNotFoundException("User not found");

        var builder = new ProfileBuilder();
        builder.AddMainData(user);
        builder.AddProfilePictures(await picturesRepository.GetPicturesByUserIdAsync(user.UserId));
        builder.AddMainProfilePicture(await picturesRepository.GetProfilePictureAsync(user.Profile?.ProfilePictureId));
        builder.AddUserInterests(await interestsRepository.GetUserInterestsByUserIdAsync(id));
        return builder.Build();
    }

    public async Task<List<User>> GetFullProfilesAsync(SearchParameters search, SortParameters sort,
        PaginationParameters pagination)
    {
        var users = await profileRepository.GetFullProfilesAsync(search, sort, pagination);
        if (users == null) return new List<User>();
        var builder = new ProfileBuilder();
        var usersList = new List<User>();
        foreach (var user in users)
        {
            builder.AddMainData(user);
            builder.AddMainProfilePicture(
                await picturesRepository.GetProfilePictureAsync(user.Profile?.ProfilePictureId));
            builder.AddUserInterests(await interestsRepository.GetUserInterestsByUserIdAsync(user.UserId));
            usersList.Add(builder.Build());
        }

        return usersList;
    }

    public async Task UpdateProfileAsync(long id, Profile profile)
    {
        var currentProfile = await profileRepository.GetProfileByIdAsync(id);
        if (currentProfile == null) throw new ObjectNotFoundException("Profile not found");
        profile.ProfileId = id;
        profile.UpdatedAt = DateTime.Now;
        var res = await profileRepository.UpdateProfileAsync(profile);
        if (res == null) throw new ObjectNotFoundException("Profile not found");
    }

    public void UploadPhoto(long id, byte[] filePicture, bool isMain)
    {
        //TODO add validation
        var isProfilePicture = isMain ? 1 : 0;
        picturesRepository.UploadPhoto(id, filePicture, isProfilePicture);
    }

    public void DeletePhoto(long userId, long photoId)
    {
        //TODO add validation
        
        picturesRepository.DeletePhoto(userId, photoId);
    }
}