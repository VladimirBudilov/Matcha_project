using AutoMapper;
using DAL.Entities;
using DAL.Helpers;
using DAL.Repositories;
using Profile = DAL.Entities.Profile;

namespace BLL.Sevices;

public class ProfileService(UserRepository userRepository, ProfileRepository profileRepository, IMapper mapper)
{
    private readonly UserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ProfileRepository _profileRepository = profileRepository;
        
    public async Task<User> GetFullProfileByIdAsync(long id)
    {
        var profile=  await _profileRepository.GetFullProfileAsync(id);
        return profile;
    }
    
    public async Task<Profile> GetProfileAsync(long id)
    {
        var profile = await _profileRepository.GetProfileByIdAsync(id);
        return profile;
    }

    public async Task<IEnumerable<User>> GetFullProfilesAsync(FilterParameters filter)
    {
        throw new NotImplementedException();
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