using DAL.Helpers;
using DAL.Repositories;
using Web_API.Helpers;

namespace BLL.Helpers;

public class ServiceValidator(
    UsersRepository usersRepository,
    ProfilesRepository profilesRepository
    )
{
    public async Task CheckUserExistence(int[] ints)
    {
        foreach (var id in ints)
        {
            var userExist = await usersRepository.GetUserByIdAsync(id);
            if (userExist == null) throw new DataValidationException($"Invalid Actor Id {id}");
        }
    }
}