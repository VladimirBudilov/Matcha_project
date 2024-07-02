using AutoMapper;
using BLL.Helpers;
using DAL.Entities;
using DAL.Helpers;
using DAL.Repositories;
using Web_API.Helpers;

namespace BLL.Sevices;

public class UserService(
    UsersRepository usersRepository,
    ProfilesRepository profilesRepository,
    IMapper mapper,
    PasswordManager passwordManager)
{
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        var output = await usersRepository.GetUserByIdAsync(userId);
        if (output == null) throw new ObjectNotFoundException("Actor not found. You can't get user that doesn't exist");

        return output;
    }

    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        var output = await usersRepository.GetUserByUserNameAsync(userName);
        if (output == null) throw new ObjectNotFoundException("Actor not found. You can't get user that doesn't exist");

        return output;
    }
    
    public async Task<User?> UpdateUserAsync(int id, User userModel)
    {
        //validate new mail that such mail doesn't exist
        var userByEmail = await usersRepository.GetUserByEmailAsync(userModel.Email);
        if (userByEmail != null && userByEmail.Id != id)
            throw new DataValidationException("User with such email already exists");
        
        var user = await usersRepository.GetUserByIdAsync(id);
        if (user == null) throw new ObjectNotFoundException("Actor not found. You can't update user that doesn't exist");
        user.UserName = userModel.UserName;
        user.FirstName = userModel.FirstName;
        user.LastName = userModel.LastName;
        user.Email = userModel.Email;

        var output = await usersRepository.UpdateUserAsync(id, userModel);
        if (output == null)
            throw new DataAccessErrorException("Actor not found. You can't update user that doesn't exist");

        return output;
    }

    public async Task<User?> DeleteUserAsync(int id)
    {
        var user = await usersRepository.GetUserByIdAsync(id);
        if (user == null) throw new ObjectNotFoundException("Actor not found. You can't delete user that doesn't exist");
        var output = await usersRepository.DeleteUserAsync(id);
        if (output == null)
            throw new ObjectNotFoundException("Actor not found. You can't delete user that doesn't exist");

        return output;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var output = await usersRepository.GetUserByEmailAsync(email);
        if (output == null)
            throw new ObjectNotFoundException(
                "Actor not found by email. You can't get user that doesn't exist by email");

        return output;
    }

    public async Task UpdatePasswordAsync(int id, string valueOldPassword, string valueNewPassword)
    {
        var user = await usersRepository.GetUserByIdAsync(id);
        if (user == null)
            throw new ObjectNotFoundException("Actor not found. You can't update password for user that doesn't exist");
        if (!passwordManager.VerifyPassword(valueOldPassword, user.Password))
            throw new DataAccessErrorException("Old password is incorrect");
        user.Password = passwordManager.HashPassword(valueNewPassword);
        var res = await usersRepository.UpdateUserAsync(id, user);
        if (res == null)
            throw new DataAccessErrorException("Actor not found. You can't update password for user that doesn't exist");
    }

    public async Task<IEnumerable<User>> GetAllUserAsync()
    {
        return await usersRepository.GetAllUsersAsync();
    }

    public async Task UpdateLastLogin(int userId)
    {
        var lastLogin = DateTime.Now;
        await usersRepository.UpdateLastLogin(userId, lastLogin);
    }
}