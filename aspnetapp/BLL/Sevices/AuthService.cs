using AutoMapper;
using BLL.Helpers;
using DAL.Entities;
using DAL.Repositories;
using Profile = DAL.Entities.Profile;

namespace BLL.Sevices;

public class AuthService(UsersRepository usersRepository,
      ProfilesRepository profilesRepository,  IMapper mapper,
    PasswordManager passwordManager, EmailService emailService)
{
    public async Task<string?> RegisterUserAsync(User user, string token)
    {
        var userByEmail = await usersRepository.GetUserByEmailAsync(user.Email);
        var userByUserName = await usersRepository.GetUserByUserNameAsync(user.UserName);
        if (userByEmail != null || userByUserName != null) return null;
        user.Password = PasswordManager.HashPassword(user.Password);
        user.EmailResetToken = token;
        user.IsVerified = false;
        user.LastLogin = DateTime.Now.ToString();
        var res = await usersRepository.CreateUserAsync(user);
        userByUserName = await usersRepository.GetUserByIdAsync((int)res);
        var profile = new Profile()
        {
            Id = userByUserName.Id,
            Gender = "male",
            Age = 18,
            ProfilePictureId = 501,
        };
        await profilesRepository.CreateProfileAsync(profile);
        return res == null ? null : user.EmailResetToken;
    }
    
    public async Task<bool> AuthenticateUser(string username, string password)
    {
        var user = await usersRepository.GetUserByUserNameAsync(username);
        if (user is not { IsVerified: true }) return false;
        return passwordManager.VerifyPassword(password, user!.Password);
    }
    
    public async Task<bool> ConfirmEmailAsync(int id)
    {
        var user = await usersRepository.GetUserByIdAsync(id);
        if(user == null) return false;
        if (!user.IsVerified) return false;
        return true;
    }
    
}