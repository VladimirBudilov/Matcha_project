using DAL.Entities;

namespace DAL.Repositories;

public class ProfileRepository
{
    public async Task<ProfileEntity> GetUserProfileByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
    
    public async Task<ProfileEntity> CreateUserProfileAsync(ProfileEntity entity)
    {
        throw new NotImplementedException();
    }
    
    public async Task<ProfileEntity> UpdateUserProfileAsync(ProfileEntity entity)
    {
        throw new NotImplementedException();
    }
    
    public async Task<ProfileEntity> DeleteUserProfileAsync(int id)
    {
        throw new NotImplementedException();
    }
    
    public async Task<IEnumerable<ProfileEntity>> GetAllUserProfilesAsync()
    {
        throw new NotImplementedException();
    }
}