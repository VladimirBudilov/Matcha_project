using AutoMapper;
using BLL.Helpers;
using DAL.Entities;
using DAL.Helpers;
using DAL.Repositories;

namespace BLL.Sevices;

public class UserService(
    UserRepository userRepository,
    ProfileRepository profileRepository,
    IMapper mapper,
    PasswordManager passwordManager)
{
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        var output = await userRepository.GetUserByIdAsync(userId);
        if (output == null) throw new ObjectNotFoundException("User not found. You can't get user that doesn't exist");

        return output;
    }

    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        var output = await userRepository.GetUserByUserNameAsync(userName);
        if (output == null) throw new ObjectNotFoundException("User not found. You can't get user that doesn't exist");

        return output;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var output = await userRepository.GetAllUsers();
        return output;
    }

    public async Task<User?> UpdateUserAsync(int id, User userModel)
    {
        var user = await userRepository.GetUserByIdAsync(id);
        if (user == null) throw new ObjectNotFoundException("User not found. You can't update user that doesn't exist");
        user.UpdatedAt = DateTime.Now;
        user.UserName = userModel.UserName;
        user.FirstName = userModel.FirstName;
        user.LastName = userModel.LastName;
        user.Email = userModel.Email;

        var output = await userRepository.UpdateUserAsync(id, userModel);
        if (output == null)
            throw new DataAccessErrorException("User not found. You can't update user that doesn't exist");

        return output;
    }

    public async Task<User?> DeleteUserAsync(int id)
    {
        var user = await userRepository.GetUserByIdAsync(id);
        if (user == null) throw new ObjectNotFoundException("User not found. You can't delete user that doesn't exist");
        var output = await userRepository.DeleteUserAsync(id);
        if (output == null)
            throw new ObjectNotFoundException("User not found. You can't delete user that doesn't exist");

        return output;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        //TODO add validation
        var output = await userRepository.GetUserByEmailAsync(email);
        if (output == null)
            throw new ObjectNotFoundException(
                "User not found by email. You can't get user that doesn't exist by email");

        return output;
    }

    public async Task UpdatePasswordAsync(int id, string valueOldPassword, string valueNewPassword)
    {
        var user = await userRepository.GetUserByIdAsync(id);
        if (user == null)
            throw new ObjectNotFoundException("User not found. You can't update password for user that doesn't exist");
        if (passwordManager.VerifyPassword(valueOldPassword, user.Password))
            throw new DataAccessErrorException("Old password is incorrect");
        user.Password = passwordManager.HashPassword(valueNewPassword);
        var res = await userRepository.UpdateUserAsync(id, user);
        if (res == null)
            throw new DataAccessErrorException("User not found. You can't update password for user that doesn't exist");
    }
}