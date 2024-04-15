using DAL.Repositories;

namespace BLL.Sevices;

public class UserProfileService
{
    private readonly UserRepository _userRepository;
        
    public UserProfileService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
        
    public async Task<IEnumerable<string>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }
}