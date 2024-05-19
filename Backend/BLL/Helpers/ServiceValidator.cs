using DAL.Helpers;
using DAL.Repositories;
using Web_API.Helpers;

namespace BLL.Helpers;

public class ServiceValidator(
    UserRepository userRepository,
    ProfileRepository profileRepository
    )
{
    public async Task CheckUserExistence(int[] ints)
    {
        foreach (var id in ints)
        {
            var userExist = await userRepository.GetUserByIdAsync(id);
            if (userExist == null) throw new DataValidationException($"Invalid User Id {id}");
        }
    }
}