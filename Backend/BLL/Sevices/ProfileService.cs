using AutoMapper;
using BLL.Helpers;
using DAL.Entities;
using DAL.Helpers;
using DAL.Repositories;
using Profile = DAL.Entities.Profile;

namespace BLL.Sevices;

public class ProfileService(UserRepository userRepository, ProfileRepository profileRepository,
    PicturesRepository picturesRepository, InterestsRepository interestsRepository,
    LikesRepository likesRepository,
    IMapper mapper)
{
    private readonly UserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ProfileRepository _profileRepository = profileRepository;
    private readonly LikesRepository _likesRepository = likesRepository;
        
    public async Task<User> GetFullProfileByIdAsync(long id)
    {
        var user = await _profileRepository.GetFullProfileAsync(id);
        if (user == null) throw new ObjectNotFoundException("User not found");
        
        var builder = new ProfileBuilder();
        builder.AddMainData(user);
        builder.AddProfilePictures(await picturesRepository.GetPicturesByUserIdAsync(user.UserId));
        builder.AddMainProfilePicture(await picturesRepository.GetProfilePictureAsync(user.Profile?.ProfilePictureId));
        builder.AddUserInterests(await interestsRepository.GetUserInterestsByUserIdAsync(id));
        return builder.Build();
    }
    
    public async Task<IEnumerable<User>> GetFullProfilesAsync(SearchParameters search, SortParameters sort,
        PaginationParameters pagination)
    {
        /*var users = await _profileRepository.GetFullProfilesAsync(search);
        if (users == null) throw new ObjectNotFoundException("Users not found");
        
        var builder = new ProfileBuilder();
        var result = new List<User>();
        foreach (var user in users)
        {
            builder.AddMainData(user);
            builder.AddProfilePictures(await _picturesRepository.GetPicturesByUserIdAsync(user.UserId));
            builder.AddMainProfilePicture(await _picturesRepository.GetProfilePictureAsync(user.Profile?.ProfilePictureId));
            builder.AddUserInterests(await _interestsRepository.GetUserInterestsByUserIdAsync(user.UserId));
            result.Add(builder.Build());
        }
        return result;*/
        return null;
    }
    
    public async Task UpdateProfileAsync(long id, Profile profile)
    {
        var currentProfile = await profileRepository.GetProfileByIdAsync(id);
        if (currentProfile == null) throw new ObjectNotFoundException("Profile not found");
        profile.ProfileId = id;
        profile.UpdatedAt = DateTime.Now;
        var res = await _profileRepository.UpdateProfileAsync(profile);
        if (res == null) throw new ObjectNotFoundException("Profile not found");
    }
}